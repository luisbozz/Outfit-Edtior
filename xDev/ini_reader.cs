using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace xDev
{
    class ini_reader
    {
        private string m_Filename;
        private string m_Section;
        private const int MAX_ENTRY = 32768;

        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileIntA", CharSet = CharSet.Ansi)]
        private static extern int GetPrivateProfileInt(string lpApplicationName, string lpKeyName, int nDefault, string lpFileName);

        [DllImport("KERNEL32.DLL", EntryPoint = "WritePrivateProfileStringA", CharSet = CharSet.Ansi)]
        private static extern int WritePrivateProfileString(string lpApplicationName, string lpKeyName, string lpString, string lpFileName);

        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileStringA", CharSet = CharSet.Ansi)]
        private static extern int GetPrivateProfileString(string lpApplicationName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileSectionNamesA", CharSet = CharSet.Ansi)]
        private static extern int GetPrivateProfileSectionNames(byte[] lpszReturnBuffer, int nSize, string lpFileName);

        [DllImport("KERNEL32.DLL", EntryPoint = "WritePrivateProfileSectionA", CharSet = CharSet.Ansi)]
        private static extern int WritePrivateProfileSection(string lpAppName, string lpString, string lpFileName);

        public ini_reader(string file)
        {
            this.Filename = file;
        }

        public string Filename
        {
            get
            {
                return this.m_Filename;
            }
            set
            {
                this.m_Filename = value;
            }
        }

        public string Section
        {
            get
            {
                return this.m_Section;
            }
            set
            {
                this.m_Section = value;
            }
        }

        public int ReadInteger(string section, string key, int defVal)
        {
            return ini_reader.GetPrivateProfileInt(section, key, defVal, this.Filename);
        }

        public int ReadInteger(string section, string key)
        {
            return this.ReadInteger(section, key, 0);
        }

        public int ReadInteger(string key, int defVal)
        {
            return this.ReadInteger(this.Section, key, defVal);
        }

        public int ReadInteger(string key)
        {
            return this.ReadInteger(key, 0);
        }

        public string ReadString(string section, string key, string defVal)
        {
            StringBuilder lpReturnedString = new StringBuilder(32768);
            ini_reader.GetPrivateProfileString(section, key, defVal, lpReturnedString, 32768, this.Filename);
            return lpReturnedString.ToString();
        }

        public string ReadString(string section, string key)
        {
            return this.ReadString(section, key, "");
        }

        public string ReadString(string key)
        {
            return this.ReadString(this.Section, key);
        }

        public long ReadLong(string section, string key, long defVal)
        {
            return long.Parse(this.ReadString(section, key, defVal.ToString()));
        }

        public long ReadLong(string section, string key)
        {
            return this.ReadLong(section, key, 0L);
        }

        public long ReadLong(string key, long defVal)
        {
            return this.ReadLong(this.Section, key, defVal);
        }

        public long ReadLong(string key)
        {
            return this.ReadLong(key, 0L);
        }

        public byte[] ReadByteArray(string section, string key)
        {
            try
            {
                return Convert.FromBase64String(this.ReadString(section, key));
            }
            catch
            {
            }
            return (byte[])null;
        }

        public byte[] ReadByteArray(string key)
        {
            return this.ReadByteArray(this.Section, key);
        }

        public bool ReadBoolean(string section, string key, bool defVal)
        {
            return bool.Parse(this.ReadString(section, key, defVal.ToString()));
        }

        public bool ReadBoolean(string section, string key)
        {
            return this.ReadBoolean(section, key, false);
        }

        public bool ReadBoolean(string key, bool defVal)
        {
            return this.ReadBoolean(this.Section, key, defVal);
        }

        public bool ReadBoolean(string key)
        {
            return this.ReadBoolean(this.Section, key);
        }

        public bool Write(string section, string key, int value)
        {
            return this.Write(section, key, value.ToString());
        }

        public bool Write(string key, int value)
        {
            return this.Write(this.Section, key, value);
        }

        public bool Write(string section, string key, string value)
        {
            return (uint)ini_reader.WritePrivateProfileString(section, key, value, this.Filename) > 0U;
        }

        public bool Write(string key, string value)
        {
            return this.Write(this.Section, key, value);
        }

        public bool Write(string section, string key, long value)
        {
            return this.Write(section, key, value.ToString());
        }

        public bool Write(string key, long value)
        {
            return this.Write(this.Section, key, value);
        }

        public bool Write(string section, string key, byte[] value)
        {
            if (value == null)
                return this.Write(section, key, (string)null);
            return this.Write(section, key, value, 0, value.Length);
        }

        public bool Write(string key, byte[] value)
        {
            return this.Write(this.Section, key, value);
        }

        public bool Write(string section, string key, byte[] value, int offset, int length)
        {
            if (value == null)
                return this.Write(section, key, (string)null);
            return this.Write(section, key, Convert.ToBase64String(value, offset, length));
        }

        public bool Write(string section, string key, bool value)
        {
            return this.Write(section, key, value.ToString());
        }

        public bool Write(string key, bool value)
        {
            return this.Write(this.Section, key, value);
        }

        public bool DeleteKey(string section, string key)
        {
            return (uint)ini_reader.WritePrivateProfileString(section, key, (string)null, this.Filename) > 0U;
        }

        public bool DeleteKey(string key)
        {
            return (uint)ini_reader.WritePrivateProfileString(this.Section, key, (string)null, this.Filename) > 0U;
        }

        public bool CreateSection(string section, string name)
        {
            return (uint)ini_reader.WritePrivateProfileSection(section, name, this.Filename) > 0U;
        }

        public bool DeleteSection(string section)
        {
            return (uint)ini_reader.WritePrivateProfileSection(section, (string)null, this.Filename) > 0U;
        }

        public ArrayList GetSectionNames()
        {
            try
            {
                byte[] numArray = new byte[32768];
                ini_reader.GetPrivateProfileSectionNames(numArray, 32768, this.Filename);
                return new ArrayList((ICollection)Encoding.ASCII.GetString(numArray).Trim(new char[1]).Split(new char[1]));
            }
            catch
            {
            }
            return (ArrayList)null;
        }
    }
}
