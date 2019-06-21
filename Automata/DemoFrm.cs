using System;
using System.Collections;
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
        string ktrabatdau = "";
        List<string> ktraketthuc = new List<string>();
        BindingSource vanphambinding = new BindingSource();
        private string[] _characters;
        private State[] _State_arr;
        string[,] trangthai = new string[100, 100];
        string chuoi = "";
        //string vanphamdau = "";
        //string vanphamsau = "";
        string vanpham = "";
        string batdau = "q0";
        //string ktraFinal = "";
        int ktraStartF = 0;
        Selectable _selectable;
        bool finalstate = true;

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
            gridAutomata.Rows.Clear();
            gridAutomata.Columns.Clear();
           // automataView.clear_ListState();
            
            _State_arr = null;
            _characters = null;
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
            // automataView.clear_ListState();
            // State.Clear_stateCollection();
            //if (_State_arr[0].Transitions.Count != 0)
            //{
            ktrabatdau = null;
            ktraketthuc = new List<string>(); ;
                for (int i = 0; i < nmrStateCount.Value; i++)
                {
                    if(automataView.IsBeginState(_State_arr[i]))
                    {
                        ktrabatdau = _State_arr[i].Label;
                    }
                    if (automataView.IsFinalState(_State_arr[i]))
                    {
                        ktraketthuc.Add(_State_arr[i].Label);
                    }
                }
                automataView.clear_ListState();
                State.Clear_stateCollection();
                _State_arr = new State[(int)nmrStateCount.Value];
                for (int i = 0; i < nmrStateCount.Value; i++)
                {
                    string stateLabel = String.Format("q{0}", i);
                    gridAutomata.Rows[i + 1].Cells[0].Value = stateLabel;
                    _State_arr[i] = new State(stateLabel);
                    automataView.States.Add(_State_arr[i]);
                }
            //}
            trangthai = new string[(int)nmrStateCount.Value, (int)_characters.Length];
            for (int i = 0; i < nmrStateCount.Value; i++)
            {
                for (int j = 0; j < _characters.Length; j++)
                {
                    String gridValue = gridAutomata.Rows[i + 1].Cells[j + 1].Value as String;
                    trangthai[i, j] = gridValue;
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
            foreach(State s in _State_arr)
            {
                if (ktraketthuc!=null)
                {
                    foreach (var item in ktraketthuc)
                    {
                        if (item == s.Label.ToString())
                        {
                            automataView.SetFinalState(s);
                        }
                    }
                }
                if(s.Label== ktrabatdau)
                {
                    automataView.SetBeginState(s);
                }
            }

            

            automataView.Refresh();
        }

        private void connectorContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            int selItemIndex = selectableContextMenu.Items.IndexOf(e.ClickedItem);
            switch (selItemIndex)
            {
                //case 0:
                //    automataView.LeaveCurveDrawingMode();              
                //    break;
                //case 1:
                //case 3:
                //    automataView.EnterCurveDrawingMode();
                //    automataView.Invalidate();
                //    break;
                case 1:
                    if (_selectable != null && _selectable is State)   // Chọn trạng thái kết thúc
                    {
                        if (automataView.IsFinalState((State)_selectable) == true) finalstate = false;
                        else finalstate = true;
                        automataView.SetFinalState((State)_selectable, finalstate);
                    }
                    automataView.Invalidate();
                    break;
                case 2:
                    if (_selectable != null && _selectable is State) //chọn trạng thái bắt đầu
                    {
                        State s = _selectable as State;
                        batdau = s.Label;
                        automataView.SetBeginState((State)_selectable);
                    }
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
                    //itemAdjustCurve.Enabled = itemCurve.Checked = _selectable is CurvedStateConnector;
                    //itemStraightLine.Checked = !itemCurve.Checked;
                    //itemFinalState.Enabled = false;
                    //selectableContextMenu.Show(automataView, e.Location);

                }
                else if (_selectable is State)
                {
                    var state = _selectable as State;
                    //itemStraightLine.Checked = itemCurve.Checked = false;
                    //itemAdjustCurve.Enabled = itemCurve.Enabled =
                    //    itemStraightLine.Enabled = false;
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
            //automataView.StartDemo(tbxInput.Text);
        }

        private void btnChuyenVanPham_Click(object sender, EventArgs e)
        {
            ktraStartF = 0;
            vanpham = "";
            string nhan = "";
            //int row=0, column=0;           
            string dau = "q0", cuoi = "";
            //string [] Chuoicuoi ;
            string vao = "q0", ra = "";
            string Madau = "";
            string Macuoi = "";
            char chuyen = 'A';
            for (int i = Convert.ToInt32((batdau[batdau.Length - 1]).ToString()); i < nmrStateCount.Value; i++)
            {
                dau = "q" + i;
                foreach (DictionaryEntry item in _State_arr[i].Transitions)
                {
                    for (int j = 0; j < _characters.Length; j++)
                    {
                        nhan = _characters[j];
                        if (item.Key.ToString() == _characters[j])
                        {
                            var list = item.Value as List<State>;
                            foreach (var item2 in list)
                            {
                                cuoi = item2.Label;
                                Macuoi = cuoi[cuoi.Length - 1].ToString();
                                Madau = dau[dau.Length - 1].ToString();
                                if (dau == batdau) vao = "S";
                                else vao = ((char)(chuyen + Convert.ToInt32(Madau))).ToString();
                                if (cuoi == batdau) ra = "S";
                                else ra = ((char)(chuyen + Convert.ToInt32(Macuoi))).ToString();
                                if (automataView.IsFinalState(item2))
                                {
                                    vanpham += "\t" + vao + "   " + "-->" + "   " + nhan + ra + " | " + nhan + "\n";
                                }
                                else
                                {
                                    if (item2.Transitions.Count != 0)
                                    {
                                        int ktraTransions = 0;
                                        foreach (DictionaryEntry transion in item2.Transitions)
                                        {
                                            int a = Convert.ToInt32((item2.Label[item2.Label.Length - 1]).ToString());
                                            int b = Convert.ToInt32(transion.Key.ToString());
                                            if (trangthai[a,b] != item2.Label) ktraTransions = 1;
                                        }
                                        if (ktraTransions == 1) vanpham += "\t" + vao + "   " + "-->" + "   " + nhan + ra + "\n";
                                    }

                                }
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < Convert.ToInt32((batdau[batdau.Length - 1]).ToString()); i++)
            {
                dau = "q" + i;
                //vanphamsau = "";
                //ktraFinal = "";
                foreach (DictionaryEntry item in _State_arr[i].Transitions)
                {
                    for (int j = 0; j < _characters.Length; j++)
                    {
                        nhan = _characters[j];
                        if (item.Key.ToString() == _characters[j])
                        {
                            var list = item.Value as List<State>;
                            foreach (var item2 in list)
                            {
                                cuoi = item2.Label;
                                Macuoi = cuoi[cuoi.Length - 1].ToString();
                                Madau = dau[dau.Length - 1].ToString();
                                if (dau == batdau) vao = "S";
                                else vao = ((char)(chuyen + Convert.ToInt32(Madau))).ToString();
                                if (cuoi == batdau) ra = "S";
                                else ra = ((char)(chuyen + Convert.ToInt32(Macuoi))).ToString();
                                //if (automataView.IsFinalState(item2))
                                //{
                                //    ktraFinal += nhan;
                                //}
                                //vanphamdau = "  " + vao + "   " + "-->" + "   ";
                                //if (vanphamsau == "") vanphamsau += nhan + ra;
                                //else vanphamsau += " | " + nhan + ra;
                                if (automataView.IsFinalState(item2))
                                {
                                    vanpham += "\t" + vao + "   " + "-->" + "   " + nhan + ra + " | " + nhan + "\n";
                                }
                                else
                                {
                                    if(item2.Transitions.Count!=0)
                                    {
                                        int ktraTransions = 0;
                                        foreach (DictionaryEntry transion in item2.Transitions)
                                        {
                                            int a = Convert.ToInt32((item2.Label[item2.Label.Length - 1]).ToString());
                                            int b = Convert.ToInt32(transion.Key.ToString());
                                            if (trangthai[a, b] != item2.Label) ktraTransions = 1;
                                        }
                                        if (ktraTransions==1) vanpham += "\t" + vao + "   " + "-->" + "   " + nhan + ra + "\n";
                                    }
                                        
                                }
                            }
                        }
                    }
                }
                //vanpham += vanphamdau + vanphamsau;
                //if (ktraFinal != "")
                //{
                //    for (int final = 0; final < ktraFinal.Length; final++)
                //    {
                //        vanpham += " | " + ktraFinal[final];
                //    }
                //}
                //vanpham += "\n";
            }
            
            foreach(State s in _State_arr)
            {
                if (automataView.IsFinalState(s) && batdau == s.Label) ktraStartF = 1;
                if(automataView.IsBeginState(s))
                {
                    if (s.Transitions.Count == 0)
                    {
                        vanpham = "Văn phạm trống!";
                        ktraStartF = 0;
                        break;
                    }
                    int ktraTransions = 0;
                    if (ktraTransions == 0)
                    {
                        foreach (DictionaryEntry transion in s.Transitions)
                        {
                            int a = Convert.ToInt32((s.Label[s.Label.Length - 1]).ToString());
                            int b = Convert.ToInt32(transion.Key.ToString());
                            if (trangthai[a, b] != s.Label) ktraTransions = 1;
                        }
                    }
                    if(ktraTransions==0)
                    {
                        vanpham = "Văn phạm trống!";
                        ktraStartF = 0;
                        break;
                    }
                }
                
            }
            if (ktraStartF == 1)
            {
                vanpham += "\tS   -->   ε";
            }
            rtxbVanPham.Clear();
            rtxbVanPham.Text = vanpham;

        }



    }
}
