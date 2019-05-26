using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AutomataLib;
namespace Automata
{
    public partial class DemoFrm : Form
    {
        BindingSource vanphambinding = new BindingSource();
        private string[] _characters;
        private State[] _State_arr;
        string[,] trangthai = new string[100,100];
        string chuoi = "";
        string vanpham = "";
        Selectable _selectable;
        public DemoFrm()
        {
            InitializeComponent();
            automataView.DemoFinished += automataView_DemoFinished;
            
        }

        void automataView_DemoFinished(object sender, DemoFinishedEvent ea)
        {
            string title = "Mô phỏng kết thúc";          
            if (InvokeRequired)
            {
                Invoke(new AutomataView.DemoFinishedHandler(automataView_DemoFinished),
                    sender, ea);
                return;
            }
            if (ea.LanguageIsAccepted)
                MessageBox.Show("Ngôn ngữ được chấp nhận bởi automat", title, 
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            else
                MessageBox.Show("Ngôn ngữ không được chấp nhận bởi automat", title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
        }

        private void btnCreateTable_Click(object sender, EventArgs e)
        {
            _characters = tbxChar.Text.Split(new char[] { ',' });
            gridAutomata.Columns.Add(string.Empty, string.Empty);
            for (int i = 0; i < _characters.Length; i++)
            {
                _characters[i] = _characters[i].Trim();
                gridAutomata.Columns.Add(string.Empty, string.Empty);
            }
            gridAutomata.Rows.Add((int)nmrStateCount.Value + 1);
            gridAutomata.Rows[0].Cells[0].Value = "Trạng thái / Kí tự vào";
            for (int i = 0; i < _characters.Length; i++)
            {
                gridAutomata.Rows[0].Cells[i + 1].Value = _characters[i];
            }
            _State_arr = new State[(int)nmrStateCount.Value];
            for (int i = 0; i < nmrStateCount.Value; i++)
            {
                string stateLabel = String.Format("q{0}", i);
                gridAutomata.Rows[i + 1].Cells[0].Value = stateLabel;
                _State_arr[i] = new State(stateLabel);
                automataView.States.Add(_State_arr[i]);
            }
            //var newWindow = new GridForm((int)nmrStateCount.Value, tbxChar.Text);
            //newWindow.ShowDialog(this);


        }

        private void gridAutomata_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex == 0 || e.ColumnIndex == 0)
                e.Cancel = true;
        }

        private void btnCreateAutomata_Click(object sender, EventArgs e)
        {
            
            trangthai = new string[(int)nmrStateCount.Value,(int)_characters.Length];
            for (int i = 0; i < nmrStateCount.Value; i++)
            {
                for (int j = 0; j < _characters.Length; j++)
                {
                    String gridValue = gridAutomata.Rows[i + 1].Cells[j + 1].Value as String;
                    trangthai[i,j] = gridValue;
                    String transitChar = gridAutomata.Rows[0].Cells[j + 1].Value as String;
                    if (gridValue != null && gridValue != String.Empty)
                    {
                        string[] stateNames = gridValue.Split(new char[] { ',' });
                        foreach (string stateName in stateNames)
                        {
                            _State_arr[i].AddTransition(transitChar[0],
                                State.GetStateFromName(stateName.Trim()));
                        }
                    }
                }
            }

            automataView.BuildAutomata();
            automataView.Refresh();
        }

        private void connectorContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            int selItemIndex = selectableContextMenu.Items.IndexOf(e.ClickedItem);
            switch (selItemIndex)
            {
                case 0:
                    automataView.LeaveCurveDrawingMode();              
                    break;
                case 1:
                case 3:
                    automataView.EnterCurveDrawingMode();
                    automataView.Invalidate();
                    break;
                case 4:
                    if (_selectable != null && _selectable is State)
                        automataView.SetFinalState((State)_selectable);
                    automataView.Invalidate();
                    break;
            }
            
        }

        private void DemoFrm_Load(object sender, EventArgs e)
        {
            automataView.ItemSelected += automataView_ItemSelected;
            KeyPreview = true;
        }

        void automataView_ItemSelected(object sender, ItemSelectInfo info)
        {
            _selectable = info.selected;
        }

        private void automataView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && _selectable != null)
            {
                if (_selectable is StateConnector)
                {
                    itemAdjustCurve.Enabled = itemCurve.Checked = _selectable is CurvedStateConnector;
                    itemStraightLine.Checked = !itemCurve.Checked;
                    itemFinalState.Enabled = false;
                    selectableContextMenu.Show(automataView, e.Location);

                }
                else if (_selectable is State)
                {
                    var state = _selectable as State;
                    itemStraightLine.Checked = itemCurve.Checked = false;
                    itemAdjustCurve.Enabled = itemCurve.Enabled =
                        itemStraightLine.Enabled = false;
                    itemFinalState.Enabled = true;
                    itemFinalState.Checked = automataView.IsFinalState(state);
                    selectableContextMenu.Show(automataView, e.Location);
                }
            }
            


        }

        private void DemoFrm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                automataView.LeaveCurveDrawingMode();
                automataView.Invalidate();
            }
        }

        private void DemoFrm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ConnectorLabel.ScreenGraphics.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            automataView.StartDemo(tbxInput.Text);
        }

        private void Chuyen(int a)
        {

        }

        private void btnChuyenVanPham_Click(object sender, EventArgs e)
        {
            //chuoi = tbxInput.Text;
            vanpham = "";
            string nhan = "";
            //int row=0, column=0;
            string dau = "q0",cuoi="";
            string [] Chuoicuoi ;
            string vao = "q0", ra = "";
            string Madau =""  ;
            string Macuoi="" ;
            for (int i = 0; i <nmrStateCount.Value ; i++)
            {
                for (int k = 0; k < _characters.Length; k++)
                {
                    nhan = _characters[k];
                    dau = "q" + i;
                    Chuoicuoi = trangthai[i,k].Split(new char[] { ',' });
                    for (int m=0;m<Chuoicuoi.Length;m++)
                    {
                        //State s = cuoi;
                        cuoi = Chuoicuoi[m];
                        char chuyen = 'A';
                        for (int j = cuoi.Length - 1; j > 0; j--)
                        {
                            Macuoi = cuoi[j].ToString();
                            break;
                        }
                        for (int j = dau.Length - 1; j > 0; j--)
                        {
                            Madau = dau[j].ToString();
                            break;
                        }
                        if (dau == "q0") vao = "S";
                        else vao = ((char)(chuyen + Convert.ToInt32(Madau) - 1)).ToString();
                        if (cuoi == "q0") ra = "S";
                        else ra = ((char)(chuyen + Convert.ToInt32(Macuoi) - 1)).ToString();
                        //string ttcuoi = automataView.IsFinalState(State s);
                        vanpham += "\t" + vao + "   " + "-->" + "   " + nhan + ra + "\n";
                    }
                    //dau = cuoi;
                }
                rtxbVanPham.Clear();
                rtxbVanPham.Text = vanpham;
            }
            //    for (int i=0; i<chuoi.Length;i++)
            //    {
            //        a = chuoi[i].ToString();
            //        //if (Convert.ToInt32(a) == 0) a = "1";
            //        //else a = "0";
            //        for (int j = vaora.Length-1;j>0;j--)
            //        {
            //            b = vaora[j].ToString();
            //            break;
            //        }
            //        for (int k = 0; k < _characters.Length; k++)
            //        {
            //            if (_characters[k] == a)
            //            {
            //                a = k.ToString();
            //                break;
            //            }
            //        }
            //        row = Convert.ToInt32(b);
            //        column = Convert.ToInt32(a);
            //        ketiep = trangthai[row, column];

            //        char chuyen = 'A';
            //        for (int j = ketiep.Length - 1; j > 0; j--)
            //        {
            //            cuoi = ketiep[j].ToString();
            //            break;
            //        }
            //        if(vaora=="q0") dau = "S";
            //        else  dau = ((char)(chuyen + Convert.ToInt32(b)-1)).ToString();
            //        if (ketiep == "q0") cuoi = "S";
            //        else cuoi = ((char)(chuyen + Convert.ToInt32(cuoi)-1)).ToString();
            //        vanpham += "\t" +dau + "   " + "-->" + "   " + _characters[Convert.ToInt32(a)]  + cuoi +"\n";
            //        vaora = ketiep;
            //    }
            //    rtxbVanPham.Clear();
            //    rtxbVanPham.Text = vanpham;
        }
    }
}
