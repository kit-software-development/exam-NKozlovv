using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace triangulation
{
    public partial class FigureForm : Form
    {
        public FigureForm()
        {
            InitializeComponent();
        }

        private void FigureForm_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "databaseDataSet.PolygonCoords". При необходимости она может быть перемещена или удалена.
            this.polygonCoordsTableAdapter.Fill(this.databaseDataSet.PolygonCoords);

        }
    }
}
