﻿using CKxlsxLib;
using CKxlsxLib.Excel;
using CKxlsxLib.Reader;
using CKxlsxLib.Writer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace ExcelReaderUnitTestProject
{
    [TestClass]
    public class xlClassImportTester
    {
        static string path = string.Format(@"{0}\{1}", Path.Combine(Environment.CurrentDirectory), "test2.xlsx");

        TestExcelClass[] data = new TestExcelClass[]
        {
            new TestExcelClass() { intProperty1 = 1 , SomeDate = DateTime.Now, SomeString = "asdasd"},
            new TestExcelClass() { intProperty1 = 2 , SomeDate = DateTime.Now, SomeString = "aafgf"},
            new TestExcelClass() { intProperty1 = 3 , SomeDate = DateTime.Now, SomeString = "xdfe"},
            new TestExcelClass() { intProperty1 = 4 , SomeDate = DateTime.Now, SomeString = "dfdr"},
            new TestExcelClass() { intProperty1 = 5 , SomeDate = DateTime.Now, SomeString = "ghdg"},
            new TestExcelClass() { intProperty1 = 7 , SomeDate = DateTime.Now, SomeString = "dfg"},
            new TestExcelClass() { intProperty1 = 9 , SomeDate = DateTime.Now, SomeString = "dfgag"},
            new TestExcelClass() { intProperty1 = 10, SomeDate = DateTime.Now, SomeString = "sdfsw"},
        };

        [ClassInitialize]
        public static void Initialize(TestContext ctx)
        {
            File.Delete(path);
        }

        [TestMethod]
        public void Write()
        {
            var err = xlWriter.Create(data).SaveToFile(path);            
            if (err.Count() > 0)
                Assert.Fail("Ошибка сохранения:\n{0}", string.Join("\n", err.Select(x => x.Description)));
        }

        [TestMethod]
        public void Read()
        {
            Write();
            var readedData = xlReader.FromFile(path).ReadToEnumerable<TestExcelClass>().ToArray();
            Assert.AreEqual(data.Count(), readedData.Count(),"Количество загруженных строк не совпадает");
            for (int i = 0; i < data.Count(); i++)
            {
                Assert.AreEqual(data[i].intProperty1, readedData[i].intProperty1, "Поля заполены не верно");
                Assert.AreEqual(data[i].intProperty2, readedData[i].intProperty2, "Поля заполены не верно");
                Assert.AreEqual(data[i].SomeDate.ToShortDateString(), readedData[i].SomeDate.ToShortDateString(), "Поля заполены не верно");
                Assert.AreEqual(data[i].SomeString, readedData[i].SomeString, "Поля заполены не верно");
            }
        }

        [TestMethod]
        public void ReadToArrayWithoutNullableColumn()
        {
            var book = new xlBook();
            var sh = book.AddSheet("sheet1");
            sh.AddCell("Поле 1", "A1", xlContentType.SharedString);
            sh.AddCell("Какая-то дата", "B1", xlContentType.SharedString);
            sh.AddCell("Мультизагаловок2", "C1", xlContentType.SharedString);
            sh.AddCell("дробь", "E1", xlContentType.SharedString);
            sh.AddCell(1, "A2", xlContentType.Integer);
            sh.AddCell(DateTime.Now, "B2", xlContentType.Date);
            sh.AddCell("Какая-то строка", "C2", xlContentType.SharedString);
            sh.AddCell("0.15", "E2", xlContentType.Double);
            sh.AddCell(2, "A3", xlContentType.Integer);
            sh.AddCell(DateTime.Now, "B3", xlContentType.Date);
            sh.AddCell("Какая-то строка", "C3", xlContentType.SharedString);
            sh.AddCell("0.25", "E3", xlContentType.Double);
            var memstream = new MemoryStream();
            xlWriter.Create(book).SaveToStream(memstream);

            TestExcelClass[] data = xlReader.FromStream(memstream).ReadToEnumerable<TestExcelClass>().ToArray();
            Assert.AreEqual(2, data.Count());
            Assert.IsTrue(data.All(x => !x.intProperty2.HasValue));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MultiCaptionTest()
        {
            var book = new xlBook();
            var sh = book.AddSheet("sheet1");
            sh.AddCell("Поле 1", "A1", xlContentType.SharedString);
            sh.AddCell("Какая-то дата", "B1", xlContentType.SharedString);
            sh.AddCell("Мультизагаловок1", "C1", xlContentType.SharedString);
            sh.AddCell("Мультизагаловок2", "D1", xlContentType.SharedString);
            sh.AddCell("дробь", "E1", xlContentType.SharedString);
            sh.AddCell(1, "A2", xlContentType.Integer);
            sh.AddCell(DateTime.Now, "B2", xlContentType.Date);
            sh.AddCell("Какая-то строка", "C2", xlContentType.SharedString);
            sh.AddCell("Какая-то строка", "D2", xlContentType.SharedString);
            sh.AddCell("0.15", "E2", xlContentType.Double);
            sh.AddCell(2, "A3", xlContentType.Integer);
            sh.AddCell(DateTime.Now, "B3", xlContentType.Date);
            sh.AddCell("Какая-то строка", "C3", xlContentType.SharedString);
            sh.AddCell("Какая-то строка", "D3", xlContentType.SharedString);
            sh.AddCell("0.25", "E3", xlContentType.Double);
            var memstream = new MemoryStream();
            xlWriter.Create(book).SaveToStream(memstream);

            TestExcelClass[] data = xlReader.FromStream(memstream).ReadToEnumerable<TestExcelClass>().ToArray();
            Assert.AreEqual(2, data.Count());
            Assert.IsTrue(data.All(x => !x.intProperty2.HasValue));
        }

        [TestMethod]
        public void EverntTest()
        {
            var book = new xlBook();
            var sh = book.AddSheet("sheet1");
            sh.AddCell("Какая-то дата", "B1", xlContentType.SharedString);
            sh.AddCell("Мультизагаловок1", "C1", xlContentType.SharedString);
            sh.AddCell("Мультизагаловок2", "D1", xlContentType.SharedString);
            sh.AddCell("дробь", "E1", xlContentType.SharedString);
            sh.AddCell(1, "A2", xlContentType.Integer);
            sh.AddCell(DateTime.Now, "B2", xlContentType.Date);
            sh.AddCell("Какая-то строка", "C2", xlContentType.SharedString);
            sh.AddCell("Какая-то строка", "D2", xlContentType.SharedString);
            sh.AddCell("0.15", "E2", xlContentType.Double);
            sh.AddCell(2, "A3", xlContentType.Integer);
            sh.AddCell(DateTime.Now, "B3", xlContentType.Date);
            sh.AddCell("Какая-то строка", "C3", xlContentType.SharedString);
            sh.AddCell("Какая-то строка", "D3", xlContentType.SharedString);
            sh.AddCell("0.25", "E3", xlContentType.Double);
            var memstream = new MemoryStream();
            xlWriter.Create(book).SaveToStream(memstream);

            xlReader.FromStream(memstream).ReadToArray<TestExcelClass>(OnValidationFailure: (s, e) => { if (!e.MissingFields.Contains("Поле 1"))Assert.Fail(); });
            TestExcelClass[] data = xlReader.FromStream(memstream).ReadToEnumerable<TestExcelClass>().ToArray();
        }
    }
}