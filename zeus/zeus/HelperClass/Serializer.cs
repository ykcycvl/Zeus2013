﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Noti.Shared
{
    public partial class Helper
    {
        public static class Serializer
        {
            private static Encoding _serializeEncoding;
            public static Encoding Charset
            {
                get
                {
                    if (_serializeEncoding == null)
                        _serializeEncoding = Encoding.GetEncoding(1251);

                    return _serializeEncoding;
                }
                set
                {
                    _serializeEncoding = value;
                }

            }
            public static byte[] SerializeToBytes(Type type, object obj)
            {
                MemoryStream memoryStream = new MemoryStream();
                var xmlns = new XmlSerializerNamespaces();
                xmlns.Add("", "");
                XmlSerializer xs = new XmlSerializer(type);
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.GetEncoding("windows-1251"));
                xs.Serialize(xmlTextWriter, obj, xmlns);
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;

                return memoryStream.ToArray();
            }

            public static string Serialize(Type type, object obj)
            {
                MemoryStream memoryStream = new MemoryStream();
                var xmlns = new XmlSerializerNamespaces();
                xmlns.Add("", "");
                XmlSerializer xs = new XmlSerializer(type);
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.GetEncoding(1251));
                xs.Serialize(xmlTextWriter, obj,xmlns);
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                string pp = Charset.GetString(memoryStream.ToArray());
                return pp;// Charset.GetString(bytes);
            }

            public static object Deserialize(Type type, string sourse)
            {
                if(sourse.LastIndexOf("<?xml") > 0)
                {
                    sourse = sourse.Remove(0, sourse.LastIndexOf("<?xml"));
                }

                XmlSerializer serializer = new XmlSerializer(type );

                return serializer.Deserialize(new MemoryStream(Charset.GetBytes(sourse)));
            }
        }
    }
}
