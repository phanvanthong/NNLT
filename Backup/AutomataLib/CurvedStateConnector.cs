using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AutomataLib
{
    public class CurvedStateConnector : StateConnector
    {

        private Point _PtBezier;
        private Size []_dMouse;
        public CurvedStateConnector(State s1, State s2)
            : base(s1, s2)
        {

            _ControlPoints = new Point[2];
            _ControlPoints[0] = AutomataView.InvalidPoint;
            _ControlPoints[1] = AutomataView.InvalidPoint;
            _dMouse = new Size[2];
        }
        private Point[] _ControlPoints;
        public Point[] ControlPoints
        {
            get { return _ControlPoints; }
        }
        public override void CalcArrow()
        {
            _bArrowInit = true;
            PointF pt1 = new PointF(ConnectedStates[0].X, ConnectedStates[0].Y);
            PointF pt2 = new PointF(ControlPoints[0].X, ControlPoints[0].Y);
            PointF pt3 = new PointF(ControlPoints[1].X, ControlPoints[1].Y);
            PointF pt4 = new PointF(ConnectedStates[1].X, ConnectedStates[1].Y);
            CubicBezier(pt1, pt2, pt3, pt4);
        }
        private void CubicBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4)
        {
            float A = pt4.Y - pt1.Y;
            float B = pt1.X - pt4.X;
            float C = pt1.Y * (-B) - pt1.X * A;
            // Ax + By + C = 0 is the line (x1,y1) - (x4,y4)

            float AB = A * A + B * B;
            // distance from (x2,y2) to the line is less than 1
            // distance from (x3,y3) to the line is less than 1
            if ((A * pt2.X + B * pt2.Y + C) *
                (A * pt2.X + B * pt2.Y + C) < AB)
                if ((A * pt3.X + B * pt3.Y + C) *
                    (A * pt3.X + B * pt3.Y + C) < AB)
                {
                    var left = DestinationState.BoundingRect.Left;
                    var right = DestinationState.BoundingRect.Right;
                    var top = DestinationState.BoundingRect.Top;
                    var bottom = DestinationState.BoundingRect.Bottom;
                    //kiểm tra xem đoạn thẳng liệu có cắt đường tròn
                    if ((pt1.X - left) * (pt4.X - left) <= 0 ||
                        (pt1.X - right) * (pt4.X - right) <= 0 ||
                        (pt1.Y - bottom) * (pt4.Y - bottom) <= 0 ||
                        (pt1.Y - top) * (pt4.Y - top) <= 0)
                    {
                        double a, b, d;
                        var ptCenter = DestinationState.Position;
                        PointF[] ptLine = new PointF[] { pt1, pt4 };
                        ptLine[0].X -= ptCenter.X;
                        ptLine[1].X -= ptCenter.X;
                        ptLine[0].Y -= ptCenter.Y;
                        ptLine[1].Y -= ptCenter.Y;
                        double denominator = Math.Sqrt(A * A + B * B);
                        a = A / denominator; b = B / denominator;
                        C = ptLine[0].Y * (-B) - ptLine[0].X * A;
                        d = -C / denominator;
                        var r = State.DisplaySize / 2;
                        double sqrt = Math.Sqrt(r * r - d * d);
                        if (Double.IsNaN(sqrt))
                            return;
                        //tìm nghiệm của phương trình đường thẳng với đường tròn
                        double x1 = a * d + b * sqrt + ptCenter.X;
                        double x2 = a * d - b * sqrt + ptCenter.X;
                        double y1 = b * d - a * sqrt + ptCenter.Y;
                        double y2 = b * d + a * sqrt + ptCenter.Y;
                        if ((x1 - pt1.X) * (x1 - pt4.X) <= 0 &&
                            (y1 - pt1.Y) * (y1 - pt4.Y) <= 0)
                        {
                            CalcTails(new Point((int)pt1.X, (int)pt1.Y),
                                new Point((int)x1, (int)y1));
                        }
                        else if ((x2 - pt1.X) * (x2 - pt4.X) <= 0 &&
                            (y2 - pt1.Y) * (y2 - pt4.Y) <= 0)
                        {
                            CalcTails(new Point((int)pt1.X, (int)pt1.Y),
                                new Point((int)x2, (int)y2));
                        }
                        //else
                        //{
                        //    System.Console.WriteLine("This exception should never be thrown");
                        //    System.Console.WriteLine("x1: {0}, y1: {1}, x2: {2}, y2: {3}",
                        //        (int)x1, (int)y1, (int)x2, (int)y2);
                        //}

                    }
                    return;
                }
            PointF pt12 = new PointF(pt1.X + pt2.X, pt1.Y + pt2.Y);
            PointF pt23 = new PointF(pt2.X + pt3.X, pt2.Y + pt3.Y);
            PointF pt34 = new PointF(pt3.X + pt4.X, pt3.Y + pt4.Y);
            PointF pt1223 = new PointF(pt12.X + pt23.X, pt12.Y + pt23.Y);
            PointF pt2334 = new PointF(pt23.X + pt34.X, pt23.Y + pt34.Y);
            PointF pt = new PointF(pt1223.X + pt2334.X, pt1223.Y + pt2334.Y);
            CubicBezier(pt1, new PointF(pt12.X / 2, pt12.Y / 2),
                    new PointF(pt1223.X / 4, pt1223.Y / 4), new PointF(pt.X / 8, pt.Y / 8));
            CubicBezier(new PointF(pt.X / 8, pt.Y / 8),
                    new PointF(pt2334.X / 4, pt2334.Y / 4), new PointF(pt34.X / 2, pt34.Y / 2),
                    pt4);
        }
        public override bool HitTest(Point pt, Graphics g)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddBezier(ConnectedStates[0].Position, _ControlPoints[0],
                _ControlPoints[1], ConnectedStates[1].Position);
            bool bReturn = false;
            if (gp.IsVisible(pt, g))
                bReturn = true;
            gp.Dispose();
            return bReturn;
        }
        public override void CalcLabelPosition()
        {
            CalcBezierPoint();
            Label.Position = BezierPoint;
        }
        public void CalcBezierPoint()
        {
            PointF pt12 = new PointF(ConnectedStates[0].X + ControlPoints[0].X,
                            ConnectedStates[1].Y + ControlPoints[0].Y);
            PointF pt23 = new PointF(ControlPoints[0].X + ControlPoints[1].X,
                ControlPoints[0].Y + ControlPoints[1].Y);
            PointF pt34 = new PointF(ControlPoints[1].X + ConnectedStates[1].X,
                ControlPoints[1].Y + ConnectedStates[1].Y);
            PointF pt1223 = new PointF(pt12.X + pt23.X, pt12.Y + pt23.Y);
            PointF pt2334 = new PointF(pt23.X + pt34.X, pt23.Y + pt34.Y);
            _PtBezier = new Point((int)((pt1223.X + pt2334.X) / 8), 
                (int)((pt1223.Y + pt2334.Y) / 8));
        }
        public Point BezierPoint
        {
            get
            {
                return _PtBezier;
            }
        }
        public override bool HandleMouseEvent(object sender, List<BaseMouseHandler> sourceChain, System.Windows.Forms.MouseEventArgs e)
        {
            if (!(sender is ConnectorLabel))
            {
                base.HandleMouseEvent(sender, sourceChain, e);
                var enumerator = sourceChain.GetEnumerator();
                if (!enumerator.MoveNext() || SourceState == DestinationState)
                {
                    _ControlPoints[0] = e.Location + _dMouse[0];
                    _ControlPoints[1] = e.Location + _dMouse[1];
                }
                
                return true;
            }
            return false;
        }
        public override bool TrackMouse(object sender, List<BaseMouseHandler> sourceChain, System.Windows.Forms.MouseEventArgs e)
        {
            if (!(sender is ConnectorLabel))
            {
                base.TrackMouse(sender, sourceChain, e);
                var enumerator = sourceChain.GetEnumerator();
                if (!enumerator.MoveNext() || SourceState == DestinationState)
                {
                    _dMouse[0] = new Size(ControlPoints[0].X - e.X, ControlPoints[0].Y - e.Y);
                    _dMouse[1] = new Size(ControlPoints[1].X - e.X, ControlPoints[1].Y - e.Y);
                }
                return true;
            }
            return false;
        }
    }
}
