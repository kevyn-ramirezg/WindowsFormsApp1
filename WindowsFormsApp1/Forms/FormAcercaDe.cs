using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1.Forms
{
    public partial class FormAcercaDe : Form
    {
        public FormAcercaDe()
        {
            InitializeComponent();
            Load += (_, __) =>
            {
                Text = "Acerca de";
                lblTitulo.Text = "Sistema de Ventas - Proyecto Final BD1";
                lblVersion.Text = $"Versión {Application.ProductVersion}";
                lblAutor.Text = "Autores: Kevyn Ramírez - Juan Mejía - Alex Gutierrez - Universidad del Quindío";
                lblDescripcion.Text = "WinForms + Oracle (ODP.NET) + RDLC. Módulos: Ventas, Créditos, Reportes, Utilidades.";
            };
            btnCerrar.Click += (_, __) => Close();
            lnkRepo.LinkClicked += (_, __) => System.Diagnostics.Process.Start("https://github.com/kevyn-ramirezg/WindowsFormsApp1");
        }
    }
}
