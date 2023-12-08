﻿using Banco.Entidades;
using Banco.Negocio;
using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SisBanca
{
    public partial class Frm_TiposTarjetas : Form
    {
        public Frm_TiposTarjetas()
        {
            InitializeComponent();
        }

        int ID_TP_TARJETA = 0;
        int Estadoguarda = 0;


        private void Estado_texto(bool lestado)
        {
            Txt_tipoTarjeta.ReadOnly = !lestado;
            Txt_limiteTarjeta.ReadOnly = !lestado;
        }

        private void Btn_salir_cliente_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Estado_restaurar(false);
            lbl_tipoTarjetas.Text = "TIPOS DE TARJETAS DE CRÉDITO";
            Frm_Dashboard a = new Frm_Dashboard();
            a.panelDashboardIcono.Visible = true;
        }

        private void Limpia_texto()
        {
            Txt_tipoTarjeta.Text   = "";
            Txt_limiteTarjeta.Text = "";
        }

        private void Estado_Botonesprincipales(bool lEstado)
        {
            this.Btn_nuevo.Enabled = lEstado;
            this.Btn_actualizar.Enabled = lEstado;
            this.Btn_eliminar.Enabled = lEstado;
            this.Btn_reporte.Enabled = lEstado;
            this.Btn_salir_cliente.Enabled = lEstado;
        }

        private void SeleccionaItem()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Dgv_principal.CurrentRow.Cells["ID_TP_TARJETA"].Value)))
            {
                MessageBox.Show("No se tiene información para visualizar", "Aviso del Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                this.ID_TP_TARJETA     = Convert.ToInt32(Dgv_principal.CurrentRow.Cells["ID_TP_TARJETA"].Value);
                Txt_tipoTarjeta.Text   = Convert.ToString(Dgv_principal.CurrentRow.Cells["NOM_TARJETA"].Value);
                Txt_limiteTarjeta.Text = Convert.ToString(Dgv_principal.CurrentRow.Cells["LIMITE"].Value);
            }
        }

        private void Formato_tipoTarjeta()
        {
            Dgv_principal.Columns[0].Visible = false;
            Dgv_principal.Columns[1].Width = 90;
            Dgv_principal.Columns[1].HeaderText = "REGISTRO";
            Dgv_principal.Columns[2].Width = 100;
            Dgv_principal.Columns[2].HeaderText = "TIPO DE TARJETA";
            Dgv_principal.Columns[3].Width = 200;
            Dgv_principal.Columns[3].HeaderText = "LIMITE";
        }

        private void Listado_tp_tarjeta(string cTexto)
        {
            try
            {
                Dgv_principal.DataSource = N_TipoTarjetas.Listado_tp_tj(cTexto);
                this.Formato_tipoTarjeta();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }

        private void Estado_Botonesprocesos(bool lEstado)
        {
            this.Btn_cancelar.Visible = lEstado;
            this.Btn_guardar.Visible = lEstado;
            this.Btn_retornar.Visible = !lEstado;
        }

        private void Btn_nuevo_Click(object sender, EventArgs e)
        {
            Estadoguarda = 1; //Nuevo Registro
            this.Estado_Botonesprincipales(false);
            this.Estado_restaurar(false);
            this.Estado_Botonesprocesos(true);
            this.Estado_texto(true);
            Txt_tipoTarjeta.Text = "";
            Txt_tipoTarjeta.ReadOnly = false;
            Tbc_principal.SelectedIndex = 1;
            lbl_tipoTarjetas.Text = "TIPOS DE TARJETAS DE CRÉDITO";
            Txt_tipoTarjeta.Focus();
        }

        private void Btn_actualizar_Click(object sender, EventArgs e)
        {
            Estadoguarda = 2; //Actualizar Registro
            this.SeleccionaItem();
            this.Estado_Botonesprincipales(false);
            this.Estado_restaurar(false);
            this.Estado_Botonesprocesos(true);
            this.Estado_texto(true);
            Tbc_principal.SelectedIndex = 1;
            Txt_tipoTarjeta.Focus();
        }

        private void Btn_eliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Dgv_principal.CurrentRow.Cells["ID_TP_TARJETA"].Value)))
            {
                MessageBox.Show("No se tiene información para Visualizar", "Aviso del Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DialogResult Opcion;
                Opcion = MessageBox.Show("¿Estás seguro de eliminar el registro seleccionado?", "Aviso del Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (Opcion == DialogResult.Yes)
                {
                    string Rpta = "";
                    this.ID_TP_TARJETA = Convert.ToInt32(Dgv_principal.CurrentRow.Cells["ID_TP_TARJETA"].Value);
                    Rpta = N_TipoTarjetas.Eliminar_tp_tj(this.ID_TP_TARJETA);
                    if (Rpta.Equals("OK"))
                    {
                        this.Listado_tp_tarjeta("%");
                        this.Estado_restaurar(false);
                        this.ID_TP_TARJETA = 0;
                        MessageBox.Show("Registro Eliminado", "Aviso del Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        private void Btn_retornar_Click(object sender, EventArgs e)
        {
            this.Estado_Botonesprocesos(false);
            Tbc_principal.SelectedIndex = 0;
        }

        private void Btn_guardar_Click(object sender, EventArgs e)
        {
            if (Txt_tipoTarjeta.Text   == String.Empty ||
                Txt_limiteTarjeta.Text == String.Empty)
            {
                MessageBox.Show("Falta ingresa datos requeridos (*)", "Aviso del Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else //Se procedería a registrar la información
            {
                string Rpta = "";
                E_TipoTarjetas oCl = new E_TipoTarjetas();

                oCl.ID_TP_TARJETA = this.ID_TP_TARJETA;
                oCl.NOM_TARJETA   = Txt_tipoTarjeta.Text.Trim();
                oCl.LIMITE        = Txt_limiteTarjeta.Text.Trim();

                Rpta = N_TipoTarjetas.Guardar_tp_tj(Estadoguarda, oCl);
                if (Rpta.Equals("OK"))
                {
                    this.Listado_tp_tarjeta("%");
                    MessageBox.Show("Los datos han sido guardados correctamente", "Aviso del Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Estadoguarda = 0; // Sin nunguna acción
                    this.Estado_Botonesprincipales(true);
                    this.Estado_Botonesprocesos(false);
                    this.Estado_texto(false);
                    Tbc_principal.SelectedIndex = 0;
                    this.ID_TP_TARJETA = 0;
                    lbl_tipoTarjetas.Text = "TIPOS DE TARJETAS DE CRÉDITO";
                }
                else
                {
                    MessageBox.Show(Rpta, "Aviso del Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Btn_cancelar_Click(object sender, EventArgs e)
        {
            this.ID_TP_TARJETA = 0;
            this.Estadoguarda = 0;
            this.Estado_texto(true);
            this.Limpia_texto();
            this.Estado_Botonesprincipales(true);
            this.Estado_Botonesprocesos(false);
            Tbc_principal.SelectedIndex = 0;
        }

        private void Btn_buscar_Click(object sender, EventArgs e)
        {
            this.Listado_tp_tarjeta(Txt_buscar.Text.Trim());
            this.Estado_restaurar(false);
        }

        private void Dgv_principal_DoubleClick(object sender, EventArgs e)
        {
            this.SeleccionaItem();
            this.Estado_Botonesprocesos(false);
            Tbc_principal.SelectedIndex = 1;
        }

        private void Frm_TiposTarjetas_Load(object sender, EventArgs e)
        {
            Listado_tp_tarjeta(Txt_buscar.Text.Trim());
            lbl_tipoTarjetas.Text = "TIPOS DE TARJETAS DE CRÉDITO";
            this.Estado_restaurar(false);
        }

        private void Dgv_principal_DoubleClick_1(object sender, EventArgs e)
        {
            this.SeleccionaItem();
            this.Estado_Botonesprocesos(false);
            Tbc_principal.SelectedIndex = 1;
        }

        private void Btn_reporte_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = string.Format("{0}.pdf", DateTime.Now.ToString("ddMMyyyyHHmmss"));

            string PaginaHTML_Texto = Properties.Resources.plantillaTipoTarjetas.ToString();
            // Generar valores aleatorios para el RUC y el Nro
            Random random = new Random();
            string ruc = random.Next(100000000, 999999999).ToString(); // RUC de 10 dígitos
            string nro = random.Next(100000, 999999).ToString(); // Nro de 6 dígitos

            // Reemplazar los marcadores de posición en el HTML
            PaginaHTML_Texto = PaginaHTML_Texto.Replace("@FECHA", DateTime.Now.ToString("dd/MM/yyyy"));
            PaginaHTML_Texto = PaginaHTML_Texto.Replace("@RUC", ruc);
            PaginaHTML_Texto = PaginaHTML_Texto.Replace("@NRO", nro);

            string filas = string.Empty;

            foreach (DataGridViewRow row in Dgv_principal.Rows)
            {
                filas += "<tr>";
                filas += "<td>" + row.Cells["FECHA_REGISTRO"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["NOM_TARJETA"].Value.ToString()    + "</td>";
                filas += "<td>" + row.Cells["LIMITE"].Value.ToString()         + "</td>";
                filas += "</tr>";
            }
            PaginaHTML_Texto = PaginaHTML_Texto.Replace("@FILAS", filas);

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                using (FileStream stream = new FileStream(savefile.FileName, FileMode.Create))
                {
                    // Creamos un nuevo documento y lo definimos como PDF
                    Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 25);

                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();
                    pdfDoc.Add(new Phrase(""));

                    // Agregamos la imagen del banner al documento
                    iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(Properties.Resources.visa_256, System.Drawing.Imaging.ImageFormat.Png);
                    img.ScaleToFit(60, 60);
                    img.Alignment = iTextSharp.text.Image.UNDERLYING;

                    img.SetAbsolutePosition(pdfDoc.LeftMargin, pdfDoc.Top - 60);
                    pdfDoc.Add(img);

                    // -----------------------------------------------------------------------
                    iTextSharp.text.Image img2 = iTextSharp.text.Image.GetInstance(Properties.Resources.BannerYT, System.Drawing.Imaging.ImageFormat.Jpeg);
                    img2.ScaleToFit(140, 60);
                    img2.Alignment = iTextSharp.text.Image.UNDERLYING;

                    // Ajusta el valor en el eje X para mover la imagen más a la derecha
                    float offsetX = 203; // Puedes ajustar este valor según tus necesidades
                    img2.SetAbsolutePosition(pdfDoc.LeftMargin + offsetX, pdfDoc.Top - 84);
                    pdfDoc.Add(img2);

                    using (StringReader sr = new StringReader(PaginaHTML_Texto))
                    {
                        XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                    }
                    this.Estado_restaurar(false);
                    pdfDoc.Close();
                    stream.Close();

                    // Mostrar mensaje de validación
                    MessageBox.Show("El Reporte se ha generado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void Listado_tipoTarjetaCaida(string cTexto)
        {
            try
            {
                Dgv_principal.DataSource = N_TipoTarjetas.Listado_tipoTarjetaCaida(cTexto);
                this.Formato_tipoTarjeta();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }

        private void Estado_restaurar(bool lestado)
        {
            btn_recuperar.Enabled = lestado;
        }

        private void btn_verEliminados_Click(object sender, EventArgs e)
        {
            this.Listado_tipoTarjetaCaida(Txt_buscar.Text.Trim());
            lbl_tipoTarjetas.Text = "TIPOS DE TARJETAS DE CRÉDITO ELIMINADAS";
            this.Estado_restaurar(true);
        }

        private void btn_recuperar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Dgv_principal.CurrentRow.Cells["ID_TP_TARJETA"].Value)))
            {
                MessageBox.Show("No se tiene información para Visualizar", "Aviso del Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DialogResult Opcion;
                Opcion = MessageBox.Show("¿Estás seguro de restablecer el registro seleccionado?", "Aviso del Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (Opcion == DialogResult.Yes)
                {
                    string Rpta = "";
                    this.ID_TP_TARJETA = Convert.ToInt32(Dgv_principal.CurrentRow.Cells["ID_TP_TARJETA"].Value);
                    Rpta = N_TipoTarjetas.Levantar_tipoTarjetaCaida(this.ID_TP_TARJETA);
                    if (Rpta.Equals("OK"))
                    {
                        this.Listado_tipoTarjetaCaida("%");
                        this.ID_TP_TARJETA = 0;
                        MessageBox.Show("Registro Levantado", "Aviso del Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        lbl_tipoTarjetas.Text = "TIPOS DE TARJETAS DE CRÉDITO";
                    }
                }
            }
        }
    }
}