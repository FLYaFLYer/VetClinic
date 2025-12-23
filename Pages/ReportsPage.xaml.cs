using ClosedXML.Excel;
using Microsoft.Win32;
using VetClinic.Data;
using VetClinic.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace VetClinic.Pages
{
    public partial class ReportsPage : Page
    {
        public ReportsPage()
        {
            InitializeComponent();
            dpStartDate.SelectedDate = DateTime.Now.AddMonths(-1);
            dpEndDate.SelectedDate = DateTime.Now;
        }

        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            if (!dpStartDate.SelectedDate.HasValue || !dpEndDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите период");
                return;
            }

            int reportType = cmbReportType.SelectedIndex;
            DateTime start = dpStartDate.SelectedDate.Value.Date;
            DateTime end = dpEndDate.SelectedDate.Value.Date.AddDays(1);

            using (var context = new VeterContext())
            {
                switch (reportType)
                {
                    case 0: // Статистика приёмов
                        GenerateVisitsReport(context, start, end);
                        break;
                    case 1: // Лекарства с низким запасом
                        GenerateLowStockReport(context);
                        break;
                    case 2: // Пациенты по видам
                        GeneratePatientsByTypeReport(context);
                        break;
                }
            }
        }

        private void GenerateVisitsReport(VeterContext context, DateTime start, DateTime end)
        {
            var visits = context.Visits
                .Include(v => v.Patient)
                .Include(v => v.Patient.Owner)
                .Include(v => v.User)
                .Where(v => v.VisitDate >= start && v.VisitDate < end)
                .ToList();

            dataGrid.ItemsSource = visits;

            var columns = new List<DataGridColumn>
            {
                new DataGridTextColumn { Header = "Дата", Binding = new System.Windows.Data.Binding("VisitDate") { StringFormat = "{0:dd.MM.yyyy HH:mm}" }, Width = 120 },
                new DataGridTextColumn { Header = "Пациент", Binding = new System.Windows.Data.Binding("Patient.Name"), Width = 120 },
                new DataGridTextColumn { Header = "Владелец", Binding = new System.Windows.Data.Binding("Patient.Owner.FullName"), Width = 150 },
                new DataGridTextColumn { Header = "Ветеринар", Binding = new System.Windows.Data.Binding("User.FullName"), Width = 150 },
                new DataGridTextColumn { Header = "Диагноз", Binding = new System.Windows.Data.Binding("Diagnosis"), Width = 200 },
                new DataGridTextColumn { Header = "Температура", Binding = new System.Windows.Data.Binding("TemperatureFormatted"), Width = 100 },
                new DataGridTextColumn { Header = "Статус", Binding = new System.Windows.Data.Binding("Status"), Width = 80 }
            };

            dataGrid.Columns.Clear();
            foreach (var column in columns)
                dataGrid.Columns.Add(column);
        }

        private void GenerateLowStockReport(VeterContext context)
        {
            var medicines = context.Medicines.ToList();

            foreach (var medicine in medicines)
            {
                medicine.TotalQuantity = context.MedicineStocks
                    .Where(ms => ms.MedicineId == medicine.Id)
                    .Sum(ms => (int?)ms.Quantity) ?? 0;
            }

            var lowStock = medicines.Where(m => m.IsLowStock).ToList();
            dataGrid.ItemsSource = lowStock;

            var columns = new List<DataGridColumn>
            {
                new DataGridTextColumn { Header = "Название", Binding = new System.Windows.Data.Binding("Name"), Width = 200 },
                new DataGridTextColumn { Header = "Категория", Binding = new System.Windows.Data.Binding("Category"), Width = 100 },
                new DataGridTextColumn { Header = "Остаток", Binding = new System.Windows.Data.Binding("TotalQuantity"), Width = 80 },
                new DataGridTextColumn { Header = "Мин. запас", Binding = new System.Windows.Data.Binding("MinStock"), Width = 80 },
                new DataGridTextColumn { Header = "Единица", Binding = new System.Windows.Data.Binding("Unit"), Width = 70 },
                new DataGridTextColumn { Header = "Статус", Binding = new System.Windows.Data.Binding("StockStatus"), Width = 100 }
            };

            dataGrid.Columns.Clear();
            foreach (var column in columns)
                dataGrid.Columns.Add(column);
        }

        private void GeneratePatientsByTypeReport(VeterContext context)
        {
            var patients = context.Patients
                .Include(p => p.AnimalType)
                .Include(p => p.Breed)
                .Include(p => p.Owner)
                .ToList();

            // Исправляем рекурсивные шаблоны для C# 7.3
            var reportData = new List<ReportData>();

            foreach (var group in patients.GroupBy(p => p.AnimalType.Name))
            {
                var data = new ReportData
                {
                    AnimalType = group.Key,
                    Count = group.Count()
                };

                // Вычисляем средний вес
                var weights = group.Where(p => p.Weight.HasValue).Select(p => p.Weight.Value).ToList();
                if (weights.Any())
                {
                    data.AvgWeight = weights.Average();
                }

                // Вычисляем средний возраст
                var ages = new List<double>();
                foreach (var patient in group)
                {
                    if (patient.BirthDate.HasValue)
                    {
                        var age = (DateTime.Now - patient.BirthDate.Value).TotalDays / 365;
                        ages.Add(age);
                    }
                }
                if (ages.Any())
                {
                    data.AvgAge = ages.Average();
                }

                reportData.Add(data);
            }

            dataGrid.ItemsSource = reportData;

            var columns = new List<DataGridColumn>
            {
                new DataGridTextColumn { Header = "Вид животного", Binding = new System.Windows.Data.Binding("AnimalType"), Width = 150 },
                new DataGridTextColumn { Header = "Количество", Binding = new System.Windows.Data.Binding("Count"), Width = 80 },
                new DataGridTextColumn { Header = "Средний вес", Binding = new System.Windows.Data.Binding("AvgWeight") { StringFormat = "{0:N2} кг" }, Width = 100 },
                new DataGridTextColumn { Header = "Средний возраст", Binding = new System.Windows.Data.Binding("AvgAge") { StringFormat = "{0:N1} лет" }, Width = 100 }
            };

            dataGrid.Columns.Clear();
            foreach (var column in columns)
                dataGrid.Columns.Add(column);
        }

        private class ReportData
        {
            public string AnimalType { get; set; }
            public int Count { get; set; }
            public decimal? AvgWeight { get; set; }
            public double? AvgAge { get; set; }
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.ItemsSource == null)
            {
                MessageBox.Show("Сначала сформируйте отчёт");
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                FileName = $"Отчет_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    ExportToExcel(saveFileDialog.FileName);
                    MessageBox.Show("Экспорт завершен успешно!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExportToExcel(string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Отчёт");

                // Получаем все строки данных
                var items = dataGrid.ItemsSource;
                if (items == null) return;

                var itemsList = new List<object>();
                foreach (var item in items)
                {
                    itemsList.Add(item);
                }

                if (!itemsList.Any()) return;

                // Заголовки
                int row = 1;
                int col = 1;

                foreach (var column in dataGrid.Columns)
                {
                    worksheet.Cell(row, col).Value = column.Header?.ToString() ?? "";
                    worksheet.Cell(row, col).Style.Font.Bold = true;
                    worksheet.Cell(row, col).Style.Fill.BackgroundColor = XLColor.LightGreen;
                    worksheet.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    col++;
                }

                // Данные
                row = 2;
                foreach (var item in itemsList)
                {
                    col = 1;
                    foreach (var column in dataGrid.Columns)
                    {
                        try
                        {
                            if (column is DataGridTextColumn textColumn)
                            {
                                var binding = textColumn.Binding as System.Windows.Data.Binding;
                                if (binding != null && !string.IsNullOrEmpty(binding.Path.Path))
                                {
                                    // Получаем значение через рефлексию
                                    string[] propertyPath = binding.Path.Path.Split('.');
                                    object currentValue = item;

                                    foreach (var propertyName in propertyPath)
                                    {
                                        if (currentValue == null) break;

                                        var property = currentValue.GetType().GetProperty(propertyName);
                                        if (property != null)
                                        {
                                            currentValue = property.GetValue(currentValue);
                                        }
                                        else
                                        {
                                            currentValue = null;
                                            break;
                                        }
                                    }

                                    if (currentValue != null)
                                    {
                                        // Форматируем значение
                                        if (currentValue is DateTime dateValue)
                                        {
                                            if (binding.StringFormat != null && binding.StringFormat.Contains("HH:mm"))
                                            {
                                                worksheet.Cell(row, col).Value = dateValue;
                                                worksheet.Cell(row, col).Style.DateFormat.Format = "dd.MM.yyyy HH:mm";
                                            }
                                            else
                                            {
                                                worksheet.Cell(row, col).Value = dateValue.ToString("dd.MM.yyyy");
                                            }
                                        }
                                        else if (currentValue is decimal || currentValue is decimal?)
                                        {
                                            worksheet.Cell(row, col).Value = Convert.ToDecimal(currentValue);
                                            if (binding.StringFormat != null && binding.StringFormat.Contains("N2"))
                                            {
                                                worksheet.Cell(row, col).Style.NumberFormat.Format = "#,##0.00";
                                            }
                                        }
                                        else if (currentValue is double || currentValue is double?)
                                        {
                                            worksheet.Cell(row, col).Value = Convert.ToDouble(currentValue);
                                            if (binding.StringFormat != null)
                                            {
                                                if (binding.StringFormat.Contains("N1"))
                                                    worksheet.Cell(row, col).Style.NumberFormat.Format = "#,##0.0";
                                                else if (binding.StringFormat.Contains("N2"))
                                                    worksheet.Cell(row, col).Style.NumberFormat.Format = "#,##0.00";
                                            }
                                        }
                                        else
                                        {
                                            worksheet.Cell(row, col).Value = currentValue.ToString();
                                        }
                                    }
                                }
                                else
                                {
                                    // Для вычисляемых свойств
                                    worksheet.Cell(row, col).Value = item.ToString();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            worksheet.Cell(row, col).Value = $"Ошибка: {ex.Message}";
                        }
                        col++;
                    }
                    row++;
                }

                // Автонастройка ширины столбцов
                worksheet.Columns().AdjustToContents();

                // Добавляем границы для лучшего вида
                var range = worksheet.Range(1, 1, row - 1, col - 1);
                range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                workbook.SaveAs(filePath);
            }
        }
    }
}