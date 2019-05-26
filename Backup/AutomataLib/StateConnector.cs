using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AutomataLib
{
    public class StateConnector : BaseMouseHandler, Selectable
    {
        protected State[] ConnectedStates;
        private Point[] _PtArrow;
        const int TOLERANCE = 6;
        protected bool _bArrowInit;
        protected ConnectorLabel _connectorLabel;
        public StateConnector(State s1, State s2)
        {
            ConnectedStates = new State[2];
            ConnectedStates[0] = s1;
            ConnectedStates[1] = s2;
            AddHandler(s1);
            AddHandler(s2);
            s1.AddHandler(this);
            s2.AddHandler(this);
            _ArrowSize = 11;
            _bArrowInit = false;
            _PtArrow = new Point[3];
            for (int i = 0; i < _PtArrow.Length; i++)
            {
                _PtArrow[i] = new Point();
            }
            _connectorLabel = new ConnectorLabel(this);
        }
        public bool IsSelected { get; set; }

        public ConnectorLabel Label
        {
            get
            {
                return _connectorLabel;
            }
        }
        public virtual bool HitTest(Point pt, Graphics g)
        {
            if (ConnectedStates[0].X == ConnectedStates[1].X)
            {
                int yMin = Math.Min(ConnectedStates[0].Y, ConnectedStates[1].Y);
                int yMax = Math.Max(ConnectedStates[0].Y, ConnectedStates[1].Y);
                Rectangle rect = Rectangle.FromLTRB(ConnectedStates[0].X - TOLERANCE,
                                                    yMin - TOLERANCE,
                                                     ConnectedStates[0].X + TOLERANCE,
                                                     yMax + TOLERANCE);
                GraphicsPath gp = new GraphicsPath();
                gp.AddRectangle(rect);
                bool bReturn = false;
                if (gp.IsVisible(pt, g))
                {
                    bReturn = true;
                }
                gp.Dispose();
                return bReturn;
            }
            Point ptMin = ConnectedStates[0].X < ConnectedStates[1].X ?
                ConnectedStates[0].Position : ConnectedStates[1].Position;
            Point ptMax = ConnectedStates[0].X < ConnectedStates[1].X ?
                ConnectedStates[1].Position : ConnectedStates[0].Position;
            //int cxLine = Math.Abs(ConnectedStates[0].X - ConnectedStates[1].X);
            //int cyLine = Math.Abs(ConnectedStates[1].Y - ConnectedStates[1].Y);
            //int cxMouse = Math.Abs(pt.X - ConnectedStates[0].X);
            //int cyMouse = Math.Abs(pt.Y - ConnectedStates[0].Y);
            int cxLine = ptMax.X - ptMin.X;
            int cyLine = ptMax.Y - ptMin.Y;
            int cxMouse = pt.X - ptMin.X;
            int cyMouse = pt.Y - ptMin.Y;
            if (Math.Abs(cyMouse - (float)cyLine / cxLine * cxMouse) < TOLERANCE)
                return true;
            return false;
        }
        public override bool HandleMouseEvent(object sender, List<BaseMouseHandler> sourceChain,
            System.Windows.Forms.MouseEventArgs e)
        {        
                /*base.HandleMouseEvent(sender, sourceChain, e);*/
            ExecuteAfter(CalcArrow);
                /*CalcArrow();*/
            return true;
        }
        public override bool TrackMouse(object sender, List<BaseMouseHandler> sourceChain, System.Windows.Forms.MouseEventArgs e)
        {
            return true;
        }
        private int _ArrowSize;
        public int ArrowSize
        {
            get
            {
                return _ArrowSize;
            }
            set
            {
                _ArrowSize = value;
                CalcArrow();

            }
        }
        protected void CalcTails(Point ptStart, Point ptEnd)
        {
            /* calculate slope of line */
            double slope = Math.Atan2(ptEnd.Y - ptStart.Y,
                                    ptEnd.X - ptStart.X);
            //ptArrow[0] is arrow head
            _PtArrow[0] = ptEnd;
            double dbl1 = slope + 5 * Math.PI / 6;
            double dbl2 = slope + 7 * Math.PI / 6;
            _PtArrow[1] = new Point()
            {
                X = _PtArrow[0].X + (int)(_ArrowSize * Math.Cos(dbl1)),
                Y = _PtArrow[0].Y + (int)(_ArrowSize * Math.Sin(dbl1))
            };
            _PtArrow[2] = new Point()
            {
                X = _PtArrow[0].X + (int)(_ArrowSize * Math.Cos(dbl2)),
                Y = _PtArrow[0].Y + (int)(_ArrowSize * Math.Sin(dbl2))
            };

        }
        public virtual void CalcArrow()
        {
            /* calculate slope of line */
            double slope = Math.Atan2(ConnectedStates[1].Y - ConnectedStates[0].Y,
                                    ConnectedStates[1].X - ConnectedStates[0].X);
            int hypotenus = State.DisplaySize / 2;
            int dx = (int)(hypotenus * Math.Cos(slope));
            int dy = (int)(hypotenus * Math.Sin(slope));
            Point ptHead = new Point(ConnectedStates[1].X - dx,
                ConnectedStates[1].Y - dy);
            _bArrowInit = true;
            CalcTails(ConnectedStates[0].Position, ptHead);

        }
        public Point[] ArrowPoints
        {
            get
            {
                if (!_bArrowInit)
                    return null;
                return _PtArrow;
            }
        }

        public State SourceState
        {
            get { return ConnectedStates[0]; }
        }
        public State DestinationState
        {
            get { return ConnectedStates[1]; }
        }
        public State GetOtherState(State state)
        {
            if (state == ConnectedStates[0])
                return ConnectedStates[1];
            else if (state == ConnectedStates[1])
                return ConnectedStates[0];
            return null;
        }
        public void AddState(State state)
        {
            if (ConnectedStates[0] == null)
            {
                ConnectedStates[0] = state;
                return;
            }
            if (ConnectedStates[1] == null)
                ConnectedStates[1] = state;
        }
        public virtual void CalcLabelPosition()
        {
            if (ConnectedStates[0] == null || ConnectedStates[1] ==null)
            {
                return;
            }
            var initPt = new Point((ConnectedStates[0].X + ConnectedStates[1].X) / 2,
                (ConnectedStates[0].Y + ConnectedStates[1].Y) / 2);
            double dx = ConnectedStates[0].X - ConnectedStates[1].X;
            double dy = ConnectedStates[0].Y - ConnectedStates[1].Y;
            if (dx == 0 || dy == 0)
            {
                if(dy == 0)
                    initPt.Offset(0, -ConnectorLabel.EstimatedHeight);
                _connectorLabel.Position = initPt;
                return;
            }       
            var slope = Math.Atan(dy / dx);
            if (slope >= 0)
                initPt.Offset(0, -ConnectorLabel.EstimatedHeight);
            _connectorLabel.Position = initPt;            

        }

    }
}
