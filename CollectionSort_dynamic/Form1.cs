using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CollectionSort_dynamic
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Font = new Font("Cascadia Code", 10);
            WindowState = FormWindowState.Maximized;

            toolStripDropDownButton1.Font = Font;
            toolStripComboBox1.Font = Font;
            toolStripComboBox2.Font = Font;

            toolStripMenuItem1.Click += (object sndr, EventArgs ea) =>
            {
                ToolStripSeparator toolStripSeparator1 = new ToolStripSeparator();
                toolStrip1.Items.Add(toolStripSeparator1);

                ToolStripButton sortAdd = (ToolStripButton)toolStrip1.Items.Add($"{toolStripComboBox1.Text}-{toolStripComboBox2.Text}");
                sortAdd.Font = Font;
                sortAdd.BackColor = Color.AliceBlue;
                sortAdd.Tag = toolStripSeparator1;

                sortAdd.Click += (object sndrSort, EventArgs eaSort) => 
                {
                    var tsiSort = (ToolStripItem)sndrSort;
                    var tssSort = (ToolStripSeparator)tsiSort.Tag;
                    toolStrip1.Items.Remove(tssSort);
                    toolStrip1.Items.Remove(tsiSort);
                };
            };
            toolStripButton1.Click += (object sndr, EventArgs ea) =>
            {
                var sortTuples = new List<Tuple<string, Example.SortOrder>>();
                foreach (var tsb in toolStrip1.Items.OfType<ToolStripButton>())
                {
                    if (tsb.Text.Contains("asc") | tsb.Text.Contains("desc"))
                    {
                        var txtSort = tsb.Text.Split('-');
                        sortTuples.Add(Tuple.Create(txtSort[0], (Example.SortOrder)Enum.Parse(typeof(Example.SortOrder), txtSort[1])));
                    }
                }
                var bindingDataSrc = (BindingSource)dataGridView1.DataSource;
                var rows = (List<SortableObject>)bindingDataSrc.DataSource;
                //foreach (var row in rows)
                //{

                //}
                rows.Sort((s1, s2) => CollectionSort_dynamic.Example.SortBy(s1, s2, sortTuples));
                var bindSrc = new BindingSource
                {
                    DataSource = rows
                };
                dataGridView1.DataSource = bindSrc;
            };

            foreach (PropertyInfo p in typeof(SortableObject).GetProperties())
            {
                var itmField = toolStripComboBox1.Items.Add(p.Name);
            }
            foreach (var enumName in Enum.GetNames(typeof(Example.SortOrder)))
            {
                if (enumName != "none")
                {
                    var itmEnum = toolStripComboBox2.Items.Add(enumName);
                }   
            }
            toolStripComboBox1.SelectedIndex = 0;
            toolStripComboBox2.SelectedIndex = 0;

            var sortables = Enumerable.Range(1, 9).Select(s => CreateRandomObject(s < 4 ? "A" : s < 7 ? "B" : "C")).ToList();
            //var sortables = new List<SortableObject>
            //{
            //    new SortableObject(){ Field1 = "A", Field2 = 8, Field3 = 2.5, Field5 = true, Field6 = 200, Field7 = 12, Field8 = "Monkey1" },
            //    new SortableObject(){ Field1 = "A", Field2 = 7, Field3 = 2.5, Field5 = true, Field6 = 200, Field7 = 12, Field8 = "Monkey2" },
            //    new SortableObject(){ Field1 = "A", Field2 = 6, Field3 = 2.5, Field5 = true, Field6 = 200, Field7 = 12, Field8 = "Monkey3" },
            //    new SortableObject(){ Field1 = "A", Field2 = 5, Field3 = 2.5, Field5 = true, Field6 = 200, Field7 = 12, Field8 = "Monkey4" },
            //    new SortableObject(){ Field1 = "B", Field2 = 3, Field3 = 2.5, Field5 = true, Field6 = 200, Field7 = 12, Field8 = "Monkey8" },
            //    new SortableObject(){ Field1 = "B", Field2 = 2, Field3 = 2.5, Field5 = true, Field6 = 200, Field7 = 12, Field8 = "Monkey9" },
            //    new SortableObject(){ Field1 = "B", Field2 = 1, Field3 = 2.5, Field5 = true, Field6 = 200, Field7 = 12, Field8 = "Monkey10" },
            //    new SortableObject(){ Field1 = "B", Field2 = 12, Field3 = 2.5, Field5 = true, Field6 = 200, Field7 = 12, Field8 = "Monkey11" },
            //    new SortableObject(){ Field1 = "C", Field2 = 820, Field3 = 2.5, Field5 = true, Field6 = 200, Field7 = 12, Field8 = "Monkey65" },
            //    new SortableObject(){ Field1 = "C", Field2 = 5, Field3 = 2.5, Field5 = true, Field6 = 200, Field7 = 12, Field8 = "Monkey23" }
            //};
            var sortBys = new List<Tuple<string, CollectionSort_dynamic.Example.SortOrder>>
            {
                Tuple.Create("Field1", Example.SortOrder.desc),
                Tuple.Create("Field2", Example.SortOrder.desc)
            };
            sortables.Sort((s1, s2) => CollectionSort_dynamic.Example.SortBy(s1, s2, sortBys));

            BindingSource bindingSource1 = new BindingSource
            {
                DataSource = sortables
            };
            dataGridView1.DataSource = bindingSource1;
        }
        private readonly static Random rnd = new Random();
        private static SortableObject CreateRandomObject(string field1)
        {
            var sortable = new SortableObject()
            {
                Field1 = field1,
                Field2 = rnd.Next(1, 100),
                Field3 = ((double)rnd.Next(1, 100)) / 5,
                Field4 = null,
                Field5 = rnd.Next(2) == 0,
                Field6 = rnd.Next(0, 1000),
                Field7 = (byte)rnd.Next(255),
                Field8 = $"Monkey_{rnd.Next(1, 100):000}"
            };
            return sortable;
        }
    }
        public class Example
    {
        public enum SortOrder { none, asc, desc }
        internal readonly static CultureInfo enUS = new CultureInfo("en-US", false);
        internal readonly static NumberStyles nbrStyles = NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint | NumberStyles.AllowCurrencySymbol | NumberStyles.AllowParentheses | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign;
        public static int SortBy(SortableObject sObj1, SortableObject sObj2, List<Tuple<string, SortOrder>> levels, int level = 0)
        {
            if (levels.Count > level)
            {
                var levelTuple = levels[level];
                var propName = levelTuple.Item1 ?? "";
                var srtOrdr = levelTuple.Item2;
                var obj1 = sObj1?.GetType()?.GetProperty(propName)?.GetValue(sObj1, null) ?? string.Empty;
                var obj2 = sObj2?.GetType()?.GetProperty(propName)?.GetValue(sObj2, null) ?? string.Empty;
                var type = typeof(string);
                foreach (PropertyInfo p in typeof(SortableObject).GetProperties())
                {
                    if (p.Name == propName)
                        type = p.PropertyType;
                }
                var lvl = SortBy(type, obj1, obj2, srtOrdr);
                if (lvl != 0)
                    return lvl;
                return SortBy(sObj1 ?? new SortableObject(), sObj2 ?? new SortableObject(), levels, level + 1);
            }
            return 0;
        }
        private static int SortBy(Type type, object obj1, object obj2, SortOrder sortOrder = SortOrder.asc)
        {
            var str1 = ((sortOrder == SortOrder.asc ? obj1 : obj2) ?? "").ToString() ?? "";
            var str2 = ((sortOrder == SortOrder.asc ? obj2 : obj1) ?? "").ToString() ?? "";

            if (type == typeof(string))
                return StringComparer.OrdinalIgnoreCase.Compare(str1, str2);
            else if (type == typeof(bool))
            {
                _ = bool.TryParse(str1, out bool b1);
                _ = bool.TryParse(str2, out bool b2);
                return b1.CompareTo(b2);
            }
            else if (type == typeof(Bitmap) | type == typeof(Image))
            {
                var img1 = ImageToBase64((Bitmap)obj1);
                var img2 = ImageToBase64((Bitmap)obj2);
                if (sortOrder == SortOrder.asc)
                    return StringComparer.OrdinalIgnoreCase.Compare(img1, img2);
                else
                    return StringComparer.OrdinalIgnoreCase.Compare(img2, img1);
            }
            else if (type == typeof(DateTime))
            {
                // 2023-08-01 12:00:00 AM
                var dateFormats = new string[] { "yyyy-MM-dd", "yyyy-MM-dd hh:mm:ss tt" };
                DateTime.TryParseExact(str1, dateFormats, enUS, DateTimeStyles.None, out DateTime d1);
                DateTime.TryParseExact(str2, dateFormats, enUS, DateTimeStyles.None, out DateTime d2);
                return d1.CompareTo(d2);
            }
            else if (type == typeof(long) | type == typeof(int) | type == typeof(short) | type == typeof(byte))
            {
                var n1 = long.Parse(str1, nbrStyles, enUS);
                var n2 = long.Parse(str2, nbrStyles, enUS);
                return n1.CompareTo(n2);
            }
            else if (type == typeof(double) | type == typeof(decimal) | type == typeof(float))
            {
                var n1 = double.Parse(str1, nbrStyles, enUS);
                var n2 = double.Parse(str2, nbrStyles, enUS);
                return n1.CompareTo(n2);
            }
            else
                return 0;
        }
        public static string ImageToBase64(Image image, ImageFormat ImageFormat = null)
        {
            if (image == null)
                return null;
            else
            {
                if (ImageFormat == null)
                    ImageFormat = ImageFormat.Bmp;
                string base64String;
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        image.Save(ms, ImageFormat);
                        byte[] imageBytes = ms.ToArray();
                        base64String = Convert.ToBase64String(imageBytes);
                    }
                    return base64String;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
    public class SortableObject
    {
        public string Field1 { get; set; } = string.Empty;
        public int Field2 { get; set; } = 0;
        public double Field3 { get; set; } = 0;
        public Image Field4 { get; set; } = null;
        public bool Field5 { get; set; } = false;
        public long Field6 { get; set; } = 0;
        public byte Field7 { get; set; } = 0;
        public string Field8 { get; set; } = string.Empty;

        public SortableObject() { }

        public override string ToString()
        {
            var fields = new List<string> { Field1, Field2.ToString(), Field3.ToString(), Field5.ToString(), Field6.ToString(), Field7.ToString(), Field8.ToString() };
            return string.Join("|", fields);
        }
    }
}
