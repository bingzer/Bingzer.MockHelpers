using Moq;
using System.ComponentModel;
using System.Data;

namespace Bingzer.MockHelpers
{
    public class MockDataReader : Mock<IDataReader>
    {
        private int _rowCounter = 0;
        private List<Dictionary<string, object>> _records = new List<Dictionary<string, object>>();

        public MockDataReader(List<Dictionary<string, object>> records, MockBehavior behavior = MockBehavior.Default)
            : base(behavior)
        {
            _records = records;

            Setup(c => c.Read()).Returns(() => Read());
            Setup(c => c.IsDBNull(It.IsAny<int>())).Returns<int>((index) => IsDBNull(index));
            Setup(c => c.GetOrdinal(It.IsAny<string>())).Returns<string>((fieldname) => GetOrdinal(fieldname));
            Setup(c => c.GetInt16(It.IsAny<short>())).Returns<short>((index) => (short)GetValue(index));
            Setup(c => c.GetInt32(It.IsAny<int>())).Returns<int>((index) => (int)GetValue(index));
            Setup(c => c.GetDecimal(It.IsAny<int>())).Returns<int>((index) => (decimal)float.Parse(GetValue(index).ToString()));
            Setup(c => c.GetString(It.IsAny<int>())).Returns<int>((index) => (string)GetValue(index));
            Setup(c => c.GetDateTime(It.IsAny<int>())).Returns<int>((index) => (DateTime)GetValue(index));
            Setup(c => c.GetBoolean(It.IsAny<int>())).Returns<int>((index) => (bool)GetValue(index));
            Setup(c => c[It.IsAny<int>()]).Returns<int>((i) => this._records[i]);
            Setup(c => c[It.IsAny<string>()]).Returns<string>((str) => this[str]);
        }

        public List<Dictionary<string, object>> Records
        {
            get { return _records; }
        }

        public DataTable GetTable()
        {
            return ToDataTable(Records);
        }

        public bool Read()
        {
            _rowCounter++;
            if (_rowCounter <= _records.Count) return true;
            return false;
        }

        public int GetOrdinal(string fieldname)
        {
            var dict = _records[_rowCounter - 1];
            var keys = dict.Keys;

            int counter = 0;
            foreach (var key in keys)
            {
                if (key == fieldname) return counter;
                ++counter;
            }
            return -1;
        }

        public object GetValue(int index)
        {
            var dict = _records[_rowCounter - 1];
            var values = dict.Values;

            int counter = 0;
            foreach (var value in values)
            {
                if (counter == index)
                    return value;
                counter++;
            }

            return null;
        }

        public bool IsDBNull(int index)
        {
            return GetValue(index) == null;
        }

        public object this[string name]
        {
            get { return _records[_rowCounter][name]; }
        }

        public static DataTable ToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
    }
}
