using DQHEditor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DQHSEGui {
    public partial class DQHSEGui : Form {
        public DQHSEGui() {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            openFileDialog1.ShowDialog();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) {
            try {
                int index = listBox1.SelectedIndex;
                Text = "DQHSEGUI - " + index + "/" + listBox1.Items.Count;
                textBox1.Text = listBox1.Items[index].ToString();
            }
            catch { }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == '\n' || e.KeyChar == '\r') {
                int index = listBox1.SelectedIndex;
                listBox1.Items[index] = textBox1.Text;
            }
        }

        LXEditor Editor;
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e) {
            listBox1.Items.Clear();
            byte[] Script = System.IO.File.ReadAllBytes(openFileDialog1.FileName);
            Editor = new LXEditor(Script);
            foreach (string str in Editor.Import())
                listBox1.Items.Add(str);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e) {
            List<string> Strings = new List<string>();
            foreach (string item in listBox1.Items)
                Strings.Add(item);
            System.IO.File.WriteAllBytes(saveFileDialog1.FileName, Editor.Export(Strings.ToArray()));
            MessageBox.Show("File Saved.");
        }

        private void extractToolStripMenuItem_Click(object sender, EventArgs e) {
            openFileDialog2.ShowDialog();
        }
        string outdir = "";
        private void openFileDialog2_FileOk(object sender, CancelEventArgs e) {
            File[] Entries = StringPackget.Open(System.IO.File.ReadAllBytes(openFileDialog2.FileName));
            outdir = openFileDialog2.FileName + "~\\";
            if (!System.IO.Directory.Exists(outdir))
                System.IO.Directory.CreateDirectory(outdir);
            foreach (File Entry in Entries) {
                string fn = outdir + Entry.Index.ToString("X8") + ".lx";
                System.IO.File.WriteAllBytes(fn, Entry.Content);
            }
            MessageBox.Show("Packget Extracted.");
        }
        
        private void repackToolStripMenuItem_Click(object sender, EventArgs e) {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.SelectedPath = outdir;
            if (folder.ShowDialog() == DialogResult.OK) {
                SaveFileDialog sav = new SaveFileDialog();
                sav.Filter = "All Str Files|*.strs|All Files|*.*";
                if (sav.ShowDialog() == DialogResult.OK) {
                    string path = folder.SelectedPath + "\\";
                    List<File> Files = new List<File>();
                    while (System.IO.File.Exists(path + Files.Count.ToString("X8") + ".lx"))
                        Files.Add(new File() {
                            Index = Files.Count,
                            Content = System.IO.File.ReadAllBytes(path + Files.Count.ToString("X8") + ".lx")
                        });
                    System.IO.File.WriteAllBytes(sav.FileName, StringPackget.Repack(Files.ToArray()));
                    MessageBox.Show("Packget Saved.");
                }
            }
        }
    }
}
