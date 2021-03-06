﻿using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XLOC.Book;
using XLOC.Reader;
using XLOC.Utility;

namespace XLOC
{
    public class XlConverter
    {
        public static XLOCReader FromStream(Stream stream, XLOCConfiguration configuration = null) => new XLOCReader((configuration ?? new XLOCConfiguration()).AddDocument(SpreadsheetDocument.Open(stream, false)));

        public static XLOCReader FromFile(string path, XLOCConfiguration configuration = null)
        {
            try { return FromStream(new MemoryStream(File.ReadAllBytes(path)), configuration); }
            catch (Exception ex) { throw new IOException(string.Format("Не удалось открыть файл {0}", path), ex); }
        }

        public static XLOCReader FromBuffer(byte[] buf, XLOCConfiguration configuration = null) => FromStream(new MemoryStream(buf), configuration);

        public static Writer.XlWriter FromEnumerable<T>(IEnumerable<T> data) => new Writer.XlArrayWriter<T>(data);
        public static Writer.XlWriter FromBook(XlBook book) => new Writer.XlBookWriter(book);
    }

    public class XLOCReader
    {
        #region Constructor
        //=================================================
        internal XLOCReader(XLOCConfiguration Configuration) => this.Configuration = Configuration;
        //=================================================
        #endregion

        #region Properties
        //=================================================
        public XLOCConfiguration Configuration { get; set; }
        //=================================================
        #endregion

        #region Methods
        //=================================================
        public IEnumerable<T> ReadToEnumerable<T>() where T : new() => new XlArrayReader(Configuration).ReadToEnumerable<T>();
        public T[] ReadToArray<T>() where T : new() => ReadToEnumerable<T>().ToArray();
        public IEnumerable<IGrouping<SheetIdentifier, T>> ReadToGroup<T>() where T : new() => new XlArrayReader(Configuration).ReadToGroup<T>();

        public XlBook ReadToBook() => new XlBookReader(Configuration).ReadToBook(Configuration.Document);
        //=================================================
        #endregion
    }
}