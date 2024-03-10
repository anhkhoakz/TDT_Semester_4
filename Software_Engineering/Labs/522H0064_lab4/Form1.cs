﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;


namespace _522H0064_lab4
{
    public partial class Form1 : Form
    {
        SqlConnection cn;
        SqlDataAdapter data;
        SqlCommand cm;
        DataTable tb;

        public Form1()
        {
            InitializeComponent();
        }

        void showHT()
        {
            cbHT.Items.Add("Tien Giang");
            cbHT.Items.Add("Thanh Hoa");
            cbHT.Items.Add("Sai Gon");
            cbHT.Items.Add("Kien Giang");

        }

        //from student table
        void showHT1()
        {
            string sql = "select distinct hometown from Students";
            data = new SqlDataAdapter(sql, cn);
            tb = new DataTable();
            data.Fill(tb);
            cbHT.DataSource = tb;
            cbHT.ValueMember = "hometown";
            cbHT.DisplayMember = "hometown";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string sql = "server = 127.0.0.1, 1433; database = InternshipManagement; uid = SA; pwd = DuyKhanh@123";
            cn = new SqlConnection(sql);
            cn.Open();
            showHT();
            //showHT1();
        }

        private void cbHT_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sql = "select * from Students where hometown like N'" + cbHT.Text + "'";
            data = new SqlDataAdapter(sql, cn);
            tb = new DataTable();
            data.Fill(tb);
            grd.DataSource = tb;
        }

        private void bAll_Click(object sender, EventArgs e)
        {
            string sql = "select * from Students";
            data = new SqlDataAdapter(sql, cn);
            tb = new DataTable();
            data.Fill(tb);
            grd.DataSource = tb;
        }

        private void bPDF_Click(object sender, EventArgs e)
        {
            if (grd.Rows.Count - 1 > 0)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "PDF (*.pdf)|*.pdf";
                sfd.FileName = "Output.pdf";
                bool fileError = false;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(sfd.FileName))
                    {
                        try
                        {
                            File.Delete(sfd.FileName);
                        }
                        catch (IOException ex)
                        {
                            fileError = true;
                            MessageBox.Show("It wasn't possible to write the data to the disk." + ex.Message);
                        }
                    }
                    if (!fileError)
                    {
                        try
                        {
                            PdfPTable pdfTable = new PdfPTable(grd.Columns.Count);
                            pdfTable.DefaultCell.Padding = 3;
                            pdfTable.WidthPercentage = 100;
                            pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;
                            //add column
                            foreach (DataGridViewColumn column in grd.Columns)
                            {
                                PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText));
                                pdfTable.AddCell(cell);
                            }

                            //add value
                            for (int i = 0; i < grd.Rows.Count - 1; ++i)
                            {
                                for (int j = 0; j < grd.Columns.Count; ++j)
                                {
                                    // string s = grd.Rows[i].Cells[j].Value.ToString();
                                    pdfTable.AddCell(grd.Rows[i].Cells[j].Value.ToString());

                                }
                            }

                            /*
                            foreach (DataGridViewRow row in grd.Rows)
                            {
                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    pdfTable.AddCell(cell.Value.ToString());
                                }
                            }
                            */
                            using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
                            {
                                Document pdfDoc = new Document(PageSize.A4, 10f, 20f, 20f, 10f);
                                PdfWriter.GetInstance(pdfDoc, stream);
                                pdfDoc.Open();
                                pdfDoc.Add(pdfTable);
                                pdfDoc.Close();
                                stream.Close();
                            }

                            MessageBox.Show("Data Exported Successfully !!!", "Info");
                            // Application.Run(sfd.FileName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error :" + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No Record To Export !!!", "Info");
            }
        }
    }
}
