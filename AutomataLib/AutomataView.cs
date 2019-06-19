using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Threading;
namespace AutomataLib
{
    public class AutomataView : PictureBox
    {
        System.Windows.Forms.Timer _demoTimer;
        private const string CONVERSION_ERROR = "Selected object cannot be converted to curve";
        private const string OBJECT_NOT_TRACKED_ERROR = "Selected object isn't in collection";
        private static Point _InvalidPoint = new Point(-1, -1);
        private const int HANDLE_SIZE = 6;
        private Bitmap _automatBitmap;
        private Graphics _automatGraphics;
        private List<State> _drawnStateList;
        State _beginState;
        List<State> _finalStates;
        private List<StateConnector> _drawnConnectors;
        private Selectable[] _selectables;
        private bool _bShow;
        private double _angleStep;
        private bool _bCurve;
        Selectable _selectedObj;
        //điểm điều khiển hiện thời cho đường cong đang được lựa chọn
        Point[] _curveControlPoints;
        private int _SelCtrlPointIndex;
        private Size _distance_CtrlPoint_Mouse;
        private Point[] _endPoints;
        private Point _paramPoint;
        private Point[] _bPoints;
        private float? _t0, _t1;
        private Color[] _ColorGradients;
        Color _paramColor;
        AutoResetEvent _event;
        State _hilightState;
        CurvedStateConnector _animateCurve;
        Random _rnd;
        /// <summary>
        /// Initializes a new instance of the AutomataView class.
        /// </summary>
        ///
        public AutomataView()
        {
            _drawnStateList = new List<State>();
            _finalStates = new List<State>();
            _drawnConnectors = new List<StateConnector>();
            _curveControlPoints = new Point[2];
            _automatBitmap = new Bitmap(SystemInformation.VirtualScreen.Width,
                SystemInformation.VirtualScreen.Height);
            _automatGraphics = Graphics.FromImage(_automatBitmap);
            _automatGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            _demoTimer = new System.Windows.Forms.Timer();
            _demoTimer.Tick += _demoTimer_Tick;
            _demoTimer.Interval = 150;
            _event = new AutoResetEvent(false);
            _endPoints = new Point[2];
            _ColorGradients = new Color[2];
            _rnd = new Random();
        }
        private static Point[] CalcParamPoint(Point[] arrPoints, float t, bool bBezier)
        {
            Point point = new Point(0, 0);
            if (bBezier)
            {
                Point[] ptBeziers = new Point[4];
                Point pt12, pt23, pt34, pt1223, pt2334, ptAll;
                pt12 = pt23 = pt34 = pt1223 = pt2334 = ptAll = new Point();
                for (int i = 0; i < 4; i++)
                {
                    ptBeziers[i] = new Point();
                }
                ptBeziers[0] = arrPoints[0];
                pt12.X = (int)((1 - t) * arrPoints[0].X + t * arrPoints[1].X);
                pt12.Y = (int)((1 - t) * arrPoints[0].Y + t * arrPoints[1].Y);
                pt23.X = (int)((1 - t) * arrPoints[1].X + t * arrPoints[2].X);
                pt23.Y = (int)((1 - t) * arrPoints[1].Y + t * arrPoints[2].Y);
                pt34.X = (int)((1 - t) * arrPoints[2].X + t * arrPoints[3].X);
                pt34.Y = (int)((1 - t) * arrPoints[2].Y + t * arrPoints[3].Y);
                pt1223.X = (int)((1 - t) * pt12.X + t * pt23.X);
                pt1223.Y = (int)((1 - t) * pt12.Y + t * pt23.Y);
                pt2334.X = (int)((1 - t) * pt23.X + t * pt34.X);
                pt2334.Y = (int)((1 - t) * pt23.Y + t * pt34.Y);
                ptAll.X = (int)((1 - t) * pt1223.X + t * pt2334.X);
                ptAll.Y = (int)((1 - t) * pt1223.Y + t * pt2334.Y);
                ptBeziers[1] = pt12;
                ptBeziers[2] = pt1223;
                ptBeziers[3] = ptAll;
                return ptBeziers;

            }
            else
            {
                point.X = (int)((1 - t) * arrPoints[0].X + t * arrPoints[1].X);
                point.Y = (int)((1 - t) * arrPoints[0].Y + t * arrPoints[1].Y);
            }
            return new Point[] { point };
        }
        void _demoTimer_Tick(object sender, EventArgs e)
        {
            if (_t0 != null)
            {
                if (_t0 <= 1)
                {
                    bool isBezier = _animateCurve != null;
                    Point[] arrPoints = isBezier ? new Point[] 
                                        {
                                            _animateCurve.SourceState.Position,
                                            _animateCurve.ControlPoints[0],
                                            _animateCurve.ControlPoints[1],
                                            _animateCurve.DestinationState.Position
                                        } : _endPoints;
                    Point[] resultPoints = CalcParamPoint(arrPoints, (float)_t0, isBezier);
                    if (isBezier)
                        _bPoints = resultPoints;
                    else
                        _paramPoint = resultPoints[0];
                    _t0 += 0.05f;
                }
                else
                {
                    _t1 = 255;
                    _t0 = null;
                }
            }
            if (_t1 != null)
            {
                int r = (int)(_ColorGradients[1].R * (255 - _t1) / 255.0 +
                    _ColorGradients[0].R * _t1 / 255.0);
                int g = (int)(_ColorGradients[1].G * (255 - _t1) / 255.0 +
                    _ColorGradients[0].G * _t1 / 255.0);
                int b = (int)(_ColorGradients[1].B * (255 - _t1) / 255.0 +
                    _ColorGradients[0].B * _t1 / 255.0);
                _paramColor = Color.FromArgb(r, g, b);
                _t1 -= 12.75f;
                if (_t1 < 0)
                {
                    _demoTimer.Stop();
                    _event.Set();
                    _hilightState = null;
                    _t0 = _t1 = null;
                    _animateCurve = null;
                }
            }
            Invalidate();
        }
        private void startTimer()
        {
            if (InvokeRequired)
                Invoke(new timerDelegate(startTimer));
            else
                _demoTimer.Start();
        }
        private delegate void timerDelegate();
        public void doBackground(object obj)
        {
            var inputString = obj as string;
            var state = _beginState;
            DemoFinishedEvent dfe = new DemoFinishedEvent();
            for (int i = 0; i < inputString.Length; i++)
            {
                if (state.Transitions.ContainsKey(inputString[i]))
                {
                    _t0 = 0;
                    _t1 = null;
                    var destinedStates = state.Transitions[inputString[i]] as List<State>;
                    _endPoints[0] = state.Position;
                    int random = 0;
                    //đối với NFA, một trạng thái tới ngẫu nhiên sẽ được chọn
                    if (destinedStates.Count > 1)
                    {
                        random = _rnd.Next(0, destinedStates.Count);
                    }
                    var destinedState = destinedStates[random];
                    _animateCurve = (CurvedStateConnector)state.Find(
                        o => o is CurvedStateConnector &&
                       ((CurvedStateConnector)(o)).DestinationState == destinedState);
                    _endPoints[1] = destinedState.Position;
                    _ColorGradients[0] = Color.FromArgb(0xCC, 0x66, 0xFF);
                    _ColorGradients[1] = Color.FromArgb(0x00, 0x66, 0xFF);
                    _hilightState = destinedState;
                    //phải được gọi trong ngữ cảnh của UI thread
                    startTimer();
                    _event.WaitOne();
                    state = destinedState;
                }
                else
                {
                    dfe.LanguageIsAccepted = false;
                    DemoFinished(this, dfe);
                    return;
                }
            }
            if (_finalStates.Contains(state))
            {
                dfe.LanguageIsAccepted = true;
                System.Console.WriteLine("Automat accepts this input string");
            }
            else
                dfe.LanguageIsAccepted = false;
            DemoFinished(this, dfe);


        }
        public void StartDemo(string inputString)
        {
            Thread bgThread = new Thread(doBackground);
            bgThread.Start(inputString);
        }
        public static Point InvalidPoint
        {
            get
            {
                return _InvalidPoint;
            }
        }

        public void EnterCurveDrawingMode()
        {
            _bCurve = true;
            CurvedStateConnector curvedConnector;
            if (!(_selectedObj is CurvedStateConnector))
            {
                if (_selectedObj is StateConnector)
                {
                    var stateConnector = _selectedObj as StateConnector;
                    int index = Array.FindIndex(_selectables, obj => obj == _selectedObj);
                    if (index != -1)
                    {
                        int labelIndex = Array.FindIndex(_selectables,
                            obj => obj == stateConnector.Label);
                        curvedConnector = new CurvedStateConnector(
                        stateConnector.SourceState,
                        stateConnector.DestinationState);
                        _selectables[index] = curvedConnector;
                        _selectables[labelIndex] = curvedConnector.Label;
                        _selectedObj = curvedConnector;
                        curvedConnector.IsSelected = true;
                    }
                    else
                        throw new InvalidOperationException(OBJECT_NOT_TRACKED_ERROR);
                }
                else
                    throw new InvalidOperationException(CONVERSION_ERROR);
            }
            curvedConnector = _selectedObj as CurvedStateConnector;
            _curveControlPoints[0] = curvedConnector.ControlPoints[0];
            _curveControlPoints[1] = curvedConnector.ControlPoints[1];

        }
        public void LeaveCurveDrawingMode()
        {
            _bCurve = false;
            _curveControlPoints[0] = _curveControlPoints[1] = _InvalidPoint;
            _SelCtrlPointIndex = -1;
        }
        public bool IsFinalState(State s)
        {
            return _finalStates.Contains(s);
        }
        public void SetFinalState(State s)
        {
            if (!_finalStates.Contains(s))
                _finalStates.Add(s);
        }

        public void SetFinalState(State s, bool enabled)
        {
            if (enabled) SetFinalState(s);
            else
            {
                _finalStates.Remove(s);
            }
        }

        public void SetBeginState(State s)
        {
            _beginState = s;

        }

        public bool IsBeginState(State s)
        {
            if (_beginState == s) return true;
            else return false;
        }

        //public void clear()
        //{
            
        //}

        public List<State> States
        {
            get
            {
                return _drawnStateList;
            }
        }
        public void BuildAutomata()
        {
            _bShow = true;
            _SelCtrlPointIndex = -1;
            _angleStep = 2 * Math.PI / _drawnStateList.Count;
            int halfWidth = DisplayRectangle.Width / 2;
            Point ptCenter = new Point(halfWidth, DisplayRectangle.Height / 2);
            //int r = halfWidth * 2 / 3;
            int r = -200;
            double a = 0;
            List<Selectable> selectableList = new List<Selectable>();

            //pre calculate States's position
            foreach (State state in _drawnStateList)
            {
                if (state.Label == "q0")
                    _beginState = state;
                state.X = (int)(r * Math.Cos(a)) + ptCenter.X;
                state.Y = (int)(r * Math.Sin(a)) + ptCenter.Y;
                selectableList.Add(state);
                foreach (DictionaryEntry de in state.Transitions)   //tạo đường nối
                {
                    var deValue = de.Value as List<State>;
                    foreach (var destinedState in deValue)
                    {
                        //try to compact State Connectors
                        StateConnector dupConnector = null;
                        if (state == destinedState)
                        {
                            //phải tìm đường nối cong sẵn có
                            dupConnector = (CurvedStateConnector)selectableList.Find
                                (obj => obj is CurvedStateConnector
                                && ((StateConnector)obj).DestinationState == destinedState
                                && ((StateConnector)obj).SourceState == state);
                        }
                        else
                            dupConnector = (StateConnector)selectableList.Find
                                (obj => obj is StateConnector &&
                                    ((StateConnector)obj).DestinationState == destinedState
                                    && ((StateConnector)obj).SourceState == state);
                        if (dupConnector != null)
                        {
                            continue;
                        }
                        else
                        {
                            StateConnector connector;
                            if (state == destinedState)
                            {
                                //nếu trạng thái đi và đến là cùng một trạng thái
                                //thì ta sẽ tạo đường cong
                                CurvedStateConnector newCurvedConnector = new CurvedStateConnector
                                                                (state, destinedState);
                                var p=new Point();
                                if (state.Y < 305)
                                {
                                    p = new Point(state.Position.X, state.Position.Y - 100);
                                }
                                else
                                {
                                    p = new Point(state.Position.X, state.Position.Y + 100);
                                }
                                var controlPoints = newCurvedConnector.ControlPoints;
                                controlPoints[0].X = p.X - 30;
                                controlPoints[0].Y = controlPoints[1].Y = p.Y;
                                controlPoints[1].X = p.X + 30;
                                connector = newCurvedConnector;
                            }
                            else
                                connector = new StateConnector(state, destinedState);
                            selectableList.Add(connector);
                            
                        }
                        
                    }

                }
                a += _angleStep;
            }
            List<Selectable> labelList = new List<Selectable>();
            foreach (Selectable _selectable in selectableList)
            {
                if (_selectable is StateConnector)
                {
                    var connector = _selectable as StateConnector;
                    connector.CalcArrow();
                    connector.CalcLabelPosition();
                    labelList.Add(connector.Label);
                }
            }
            selectableList.AddRange(labelList);
            selectableList.Sort(CompareSelectables);
            _selectables = selectableList.ToArray();
        }
        private static bool HitHandle(Point handlePoint, Point pt)
        {
            GraphicsPath gp = new GraphicsPath();
            var rect = new Rectangle(handlePoint, new Size(HANDLE_SIZE, HANDLE_SIZE));
            rect.Offset(-HANDLE_SIZE / 2, -HANDLE_SIZE / 2);
            gp.AddEllipse(rect);
            bool bReturn = false;
            if (gp.IsVisible(pt))
                bReturn = true;
            gp.Dispose();
            return bReturn;
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (_bCurve)
            {
                if (_curveControlPoints[0] == _InvalidPoint)
                {
                    _curveControlPoints[0] = e.Location;
                    Invalidate();
                }
                else if (_curveControlPoints[1] == _InvalidPoint)
                {
                    //người dùng đã xong việc nhập điểm điều khiển
                    _curveControlPoints[1] = e.Location;
                    var curveConnector = _selectedObj as CurvedStateConnector;
                    curveConnector.ControlPoints[0] = _curveControlPoints[0];
                    curveConnector.ControlPoints[1] = _curveControlPoints[1];
                    curveConnector.CalcArrow();
                    curveConnector.CalcLabelPosition();
                    Invalidate();
                }
                else
                {
                    _SelCtrlPointIndex = -1;
                    if (HitHandle(_curveControlPoints[0], e.Location))
                        _SelCtrlPointIndex = 0;
                    if (HitHandle(_curveControlPoints[1], e.Location))
                        _SelCtrlPointIndex = 1;
                    if (_SelCtrlPointIndex != -1)
                    {
                        var selCtrlPt = _curveControlPoints[_SelCtrlPointIndex];
                        _distance_CtrlPoint_Mouse = new Size(selCtrlPt.X - e.Location.X,
                            selCtrlPt.Y - e.Location.Y);
                    }
                }
                return;
            }
            for (int i = _selectables.Length - 1; i >= 0; i--)
            {
                var _selectable = _selectables[i];
                if (_selectable.HitTest(e.Location, _automatGraphics))
                {
                    _selectable.IsSelected = true;
                    ItemSelectInfo info = new ItemSelectInfo
                    {
                        deselected = _selectedObj,
                        selected = _selectable,
                        location = e.Location

                    };
                    ItemSelected(this, info);

                    if (_selectable is CurvedStateConnector)
                    {
                        var curvedConnector = _selectable as CurvedStateConnector;
                        _curveControlPoints[0] = curvedConnector.ControlPoints[0];
                        _curveControlPoints[1] = curvedConnector.ControlPoints[1];
                    }

                    if (_selectedObj != null && _selectedObj != _selectable)
                    {
                        _selectedObj.IsSelected = false;
                    }
                    _selectedObj = _selectable;
                    List<BaseMouseHandler> sourceChain = new List<BaseMouseHandler>();
                    var mouseHandler = _selectedObj as BaseMouseHandler;
                    /*mouseHandler.TrackMouse(this, sourceChain, e);*/
                    mouseHandler.InitiateTrackMouse(this, sourceChain, e);
                    Invalidate();
                    break;
                }
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_bCurve)
            {
                if (_SelCtrlPointIndex != -1)
                {
                    _SelCtrlPointIndex = -1;
                }
                return;
            }
            if (_selectedObj != null)
            {
                if (!_selectedObj.HitTest(e.Location, _automatGraphics))
                {
                    _selectedObj.IsSelected = false;
                    ItemSelectInfo info = new ItemSelectInfo
                    {
                        deselected = _selectedObj,
                        selected = null,
                        location = e.Location

                    };
                    ItemSelected(this, info);
                    Invalidate();
                    _selectedObj = null;
                }
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_bCurve)
            {
                if (_SelCtrlPointIndex != -1 && _curveControlPoints[0] != _InvalidPoint
                    && _curveControlPoints[1] != _InvalidPoint)
                {
                    _curveControlPoints[_SelCtrlPointIndex].X = e.X +
                        _distance_CtrlPoint_Mouse.Width;
                    _curveControlPoints[_SelCtrlPointIndex].Y = e.Y +
                        _distance_CtrlPoint_Mouse.Height;
                    if (_selectedObj is CurvedStateConnector)
                    {
                        var curvedConnector = _selectedObj as CurvedStateConnector;
                        curvedConnector.ControlPoints[0] = _curveControlPoints[0];
                        curvedConnector.ControlPoints[1] = _curveControlPoints[1];
                        curvedConnector.CalcArrow();
                        curvedConnector.CalcBezierPoint();
                        /*Refresh();*/
                        Invalidate();
                    }
                }
                return;
            }
            if (e.Button == MouseButtons.Left && _selectedObj != null)
            {
                List<BaseMouseHandler> sourceChain = new List<BaseMouseHandler>();
                var mouseHandler = _selectedObj as BaseMouseHandler;
                /*mouseHandler.HandleMouseEvent(this, sourceChain, e);*/
                mouseHandler.InitiateHandleMouse(this, sourceChain, e);
                Refresh();
            }
        }
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            if (!_bShow) return;
            Font stateFont = new Font("Arial", 10, FontStyle.Bold, GraphicsUnit.Point);
            StringFormat strFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            Pen borderPen = new Pen(Color.ForestGreen, 1);
            Pen redPen = new Pen(Color.Red, 2);
            Pen connectorPen = new Pen(Color.FromArgb(0x33, 0x00, 0x33), 1);
            Brush arrowBrush = new SolidBrush(Color.FromArgb(0x33, 0x00, 0x33));

            foreach (Selectable _selectable in _selectables)
            {
                CurvedStateConnector curvedConnector = null;
                if (_selectable is StateConnector)
                {
                    var connector = _selectable as StateConnector;
                    if (connector.Label.IsSelected)
                    {
                        Pen labelBorderPen = new Pen(Color.DarkRed, 1.5f) { DashStyle = DashStyle.Dash };
                        var labelRect = connector.Label.GetRect(_automatGraphics);
                        _automatGraphics.DrawRectangle(labelBorderPen, labelRect.Left,
                            labelRect.Top, labelRect.Width, labelRect.Height);
                        labelBorderPen.Dispose();
                    }
                    Pen pen = connector.IsSelected ? redPen : connectorPen;
                    if (_selectable is CurvedStateConnector)
                    {
                        curvedConnector = _selectable as CurvedStateConnector;
                    }
                    if (curvedConnector != null && 
                        curvedConnector.ControlPoints[0] != _InvalidPoint && 
                        curvedConnector.ControlPoints[1] != _InvalidPoint)
                        {
                            _automatGraphics.DrawBezier(pen, curvedConnector.SourceState.Position,
                        curvedConnector.ControlPoints[0],
                        curvedConnector.ControlPoints[1],
                        curvedConnector.DestinationState.Position);
                            _automatGraphics.DrawString(curvedConnector.Label.Text, ConnectorLabel.LabelFont,
                         Brushes.Brown, connector.Label.Position);
                        }
                    else
                    {
                        _automatGraphics.DrawLine(pen, connector.SourceState.Position,
                            connector.DestinationState.Position);
                        _automatGraphics.DrawString(connector.Label.Text, ConnectorLabel.LabelFont,
                     Brushes.Brown, connector.Label.Position);
                    }
                    if (connector.ArrowPoints != null)
                    {
                        _automatGraphics.DrawLine(pen, connector.ArrowPoints[0],
                            connector.ArrowPoints[1]);
                        _automatGraphics.DrawLine(pen, connector.ArrowPoints[0],
                           connector.ArrowPoints[2]);     
                    }
                    
                 
                } else if(_selectable is State)
                {
                    
                    var state = _selectable as State;
                    GraphicsPath gp = new GraphicsPath();
                    gp.AddEllipse(state.BoundingRect);
                    var boundColor = (_selectable == _hilightState && _t1 >= 0 
                        && _t1 <= 255) ? _paramColor :
                        Color.FromArgb(0xCC, 0x66, 0xFF);
                    Color[] colors = {
   boundColor,    // dark green
   Color.FromArgb(0xCC, 0xCC, 0xFF),  // aqua
   Color.FromArgb(0xCC, 0xFF, 0xFF)};   // blue

                    float[] relativePositions = {
   0f,       // Dark green is at the boundary of the triangle.
   0.58f,     // Aqua is 40 percent of the way from the boundary
             // to the center point.
   1.0f};    // Blue is at the center point.

                    ColorBlend colorBlend = new ColorBlend();
                    colorBlend.Colors = colors;
                    colorBlend.Positions = relativePositions;

                    PathGradientBrush grd = new PathGradientBrush(gp);
                    //{
                    //    SurroundColors = new Color[] { Color.FromArgb(0xCC, 0x33, 0xCC) },
                    //    CenterColor = Color.FloralWhite
                    //};
                    grd.InterpolationColors = colorBlend;
                    _automatGraphics.FillEllipse(grd, state.BoundingRect);
                    _automatGraphics.DrawString(state.Label, stateFont, Brushes.DarkSlateBlue,
                        state.BoundingRect, strFormat);
                    Pen stateBorderPen = state.IsSelected ? redPen : borderPen;
                    _automatGraphics.DrawEllipse(stateBorderPen, state.BoundingRect);
                    
                    
                    if (IsFinalState(state))
                    {
                        Pen markPen = new Pen(Color.Maroon, 4);
                        var newRect = state.BoundingRect;
                        newRect.Inflate(3, 3);
                        _automatGraphics.DrawEllipse(markPen, newRect);
                        markPen.Dispose();
                    }
                    if (_beginState == state)
                    {
                        Pen pen_draw = new Pen(Color.Black);
                        Point[] pnt = new Point[3];

                        if (state.X < 305)
                        {
                            pnt[0].X = state.X - 60;
                            pnt[0].Y = state.Y - 15;
                            pnt[1].X = state.X - 60;
                            pnt[1].Y = state.Y + 15;
                            pnt[2].X = state.X - 30;
                            pnt[2].Y = state.Y;
                        }
                        else if (state.X > 305)
                        {
                            pnt[0].X = state.X + 60;
                            pnt[0].Y = state.Y - 15;
                            pnt[1].X = state.X + 60;
                            pnt[1].Y = state.Y + 15;
                            pnt[2].X = state.X + 30;
                            pnt[2].Y = state.Y;
                        }
                        _automatGraphics.DrawPolygon(pen_draw, pnt);
                    }
                    
                    grd.Dispose();
                    gp.Dispose();
                }
            }
            if (_bCurve)
            {
                if(_curveControlPoints[0] != InvalidPoint)
                    OnPaintHandle(_curveControlPoints[0]);
                if (_curveControlPoints[1] != InvalidPoint)
                    OnPaintHandle(_curveControlPoints[1]);
            }
            OnAnimatePaint(_automatGraphics);
            pe.Graphics.DrawImage(_automatBitmap, DisplayRectangle, DisplayRectangle,
                GraphicsUnit.Pixel);
            stateFont.Dispose();
            strFormat.Dispose();
            borderPen.Dispose();
            redPen.Dispose();
            connectorPen.Dispose();
            arrowBrush.Dispose();
        }

        private void OnAnimatePaint(Graphics g)
        {
            if (!_demoTimer.Enabled) return;
            if(_t0 != null)
            {
                Pen pen = new Pen(Color.DarkRed, 2);
                if(_animateCurve != null)
                    g.DrawBezier(pen, _bPoints[0], _bPoints[1], _bPoints[2], _bPoints[3]);
                else
                    g.DrawLine(pen, _endPoints[0], _paramPoint);
                pen.Dispose();

            }
        }
        private void OnPaintHandle(Point pt)
        {
            var rect = new Rectangle(pt, new Size(HANDLE_SIZE, HANDLE_SIZE));
            rect.Offset(-HANDLE_SIZE / 2, -HANDLE_SIZE / 2);
            _automatGraphics.FillEllipse(Brushes.BlueViolet, rect);
        }
        private static int CompareSelectables(Selectable s1, Selectable s2)
        {
            //các connector sẽ được sắp xếp trước
            int a = s1 is StateConnector ? 0 : (s1 is State ? 1 : 2);
            int b = s2 is StateConnector ? 0 : (s2 is State ? 1 : 2);
            return a - b;
        }
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);
            SolidBrush newSolidBrush = new SolidBrush(BackColor);
            _automatGraphics.FillRectangle(newSolidBrush, 
                pevent.ClipRectangle);
            newSolidBrush.Dispose();
        }
        public delegate void DemoFinishedHandler(object sender, DemoFinishedEvent ea);
        public delegate void ItemSelectedHandler(object sender, ItemSelectInfo info);
        public event ItemSelectedHandler ItemSelected;
        public event DemoFinishedHandler DemoFinished;
    }
}
