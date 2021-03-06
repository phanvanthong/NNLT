using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
namespace AutomataLib
{
    public class ConnectorLabel : BaseMouseHandler, Selectable
    {
        private const string FATAL_EXCEPTION = "Exception is thrown in an unexpected way";
        int _mouse_dx, _mouse_dy;
        private StringBuilder _StringBuilder;
        private static Font _LabelFont = new Font("Arial", 10, 
            FontStyle.Bold , GraphicsUnit.Point);
        private StateConnector _connector;
        private static Graphics _ScreenGraphics = Graphics.FromHdc(GetDC(IntPtr.Zero));
        private double _dAngle;
        private double _hypotenus;
        private Point _anchorPoint, _movePoint;
        private Size _OffsetFromBezier;
        public ConnectorLabel(StateConnector connector)
        {
            connector.AddHandler(this);
            AddHandler(connector);
            _connector = connector;
            _OffsetFromBezier = new Size();
            _StringBuilder = new StringBuilder(50);

            foreach (DictionaryEntry de in _connector.SourceState.Transitions)
            {
                var deValue = de.Value as List<State>;
                foreach (var destinedState in deValue)
                {
                    if (destinedState == _connector.DestinationState)
                    {
                        if (_StringBuilder.ToString().Length > 0)
                            _StringBuilder.Append(", ");
                        _StringBuilder.Append((char)de.Key);
                        break;
                    }
                }
            }
            
        }

        public override bool TrackMouse(object sender, List<BaseMouseHandler> sourceChain, System.Windows.Forms.MouseEventArgs e)
        {
            var enumerator = sourceChain.GetEnumerator();
            double current;
            if (!enumerator.MoveNext())
            {
                _mouse_dx = e.X - _Position.X;
                _mouse_dy = e.Y - _Position.Y;
                return true;
            }       
            if(_connector is CurvedStateConnector)
            {
                var curveConnector = _connector as CurvedStateConnector;
                curveConnector.CalcBezierPoint();
                _OffsetFromBezier = new Size(_Position.X - curveConnector.BezierPoint.X, 
                    _Position.Y - curveConnector.BezierPoint.Y);                
            } else
            {
                if (enumerator.Current is StateConnector)
                {
                    var connector1 = enumerator.Current as StateConnector;
                    if (connector1 == _connector ||
                        (connector1.GetOtherState(_connector.SourceState) != null &&
                        connector1.GetOtherState(_connector.DestinationState) != null))
                    {
                        _mouse_dx = e.X - _Position.X;
                        _mouse_dy = e.Y - _Position.Y;
                        return true;
                    }
                }
                double slope;
                if (sourceChain.IndexOf(_connector.SourceState) < 
                    sourceChain.IndexOf(_connector))
                {
                    _anchorPoint = _connector.DestinationState.Position;
                    _movePoint = _connector.SourceState.Position;

                }
                else if (sourceChain.IndexOf(_connector.DestinationState) < 
                    sourceChain.IndexOf(_connector))
                {
                    _anchorPoint = _connector.SourceState.Position;
                    _movePoint = _connector.DestinationState.Position;

                }
                else
                    throw new InvalidOperationException(
                        FATAL_EXCEPTION);
                slope = Math.Atan2(_movePoint.Y - _anchorPoint.Y, _movePoint.X - _anchorPoint.X);
                var dy = _Position.Y - _anchorPoint.Y;
                var dx = _Position.X - _anchorPoint.X;
                current = Math.Atan2(dy, dx);
                _hypotenus = Math.Sqrt(dx * dx + dy * dy);
                _dAngle = current - slope;
            }

           /* base.TrackMouse(sender, sourceChain, e);*/
            return true;
            
        }
        public override bool HandleMouseEvent(object sender, List<BaseMouseHandler> sourceChain, System.Windows.Forms.MouseEventArgs e)
        {
            var enumerator = sourceChain.GetEnumerator();
            //tôi có được nhận sự kiện này đầu tiên
            if (!enumerator.MoveNext())
            {
                _Position.X = e.X - _mouse_dx;
                _Position.Y = e.Y - _mouse_dy;
                //if (_connector is CurvedStateConnector)
                //{
                //    var curveConnector = _connector as CurvedStateConnector;
                //    _OffsetFromBezier = new Size(_Position.X - curveConnector.BezierPoint.X, _Position.Y - curveConnector.BezierPoint.Y);
                //}
                return true;
            }    
           
            if(_connector is CurvedStateConnector)
            {
                var curveConnector = _connector as CurvedStateConnector;
                curveConnector.CalcBezierPoint();
                _Position = curveConnector.BezierPoint + _OffsetFromBezier;                
            } else
            {
                if (enumerator.Current is StateConnector)
                {
                    var connector1 = enumerator.Current as StateConnector;
                    if (connector1 == _connector ||
                        (connector1.GetOtherState(_connector.SourceState) != null &&
                        connector1.GetOtherState(_connector.DestinationState) != null))
                    {
                        _Position.X = e.X - _mouse_dx;
                        _Position.Y = e.Y - _mouse_dy;
                        return true;
                    }
                }
                if (sourceChain.IndexOf(_connector.SourceState) <
                    sourceChain.IndexOf(_connector))
                {
                    _movePoint = _connector.SourceState.Position;

                }
                else if (sourceChain.IndexOf(_connector.DestinationState) <
                    sourceChain.IndexOf(_connector))
                {
                    _movePoint = _connector.DestinationState.Position;
                }
                else
                    throw new InvalidOperationException(
                        FATAL_EXCEPTION);
                var slope = Math.Atan2(_movePoint.Y - _anchorPoint.Y,
                    _movePoint.X - _anchorPoint.X);
                var current = slope + _dAngle;
                _Position.X = (int)(_hypotenus * Math.Cos(current) + _anchorPoint.X);
                _Position.Y = (int)(_hypotenus * Math.Sin(current) + _anchorPoint.Y);
            }

            /*base.HandleMouseEvent(sender, sourceChain, e);*/
            return true;   
        }
        public string Text 
        { 
            get
            {
                return _StringBuilder.ToString();
            }

        }
        private Point _Position;
        public Point Position
        {
            get
            {
                return _Position;
            }
            set
            {
                _Position = value;

            }
        }
        public static Font LabelFont
        {
            get
            {
                return _LabelFont;
            }
        }
        public bool IsSelected { get; set; }
        public RectangleF GetRect(Graphics g)
        {
            var measureString = g.MeasureString(Text, _LabelFont);
            var rectf = new RectangleF(Position, measureString);
            return rectf;
        }
        public bool HitTest(Point pt, Graphics g)
        {
            RectangleF rectf = GetRect(g);
            return rectf.Contains(pt);
        }
        public static int EstimatedHeight
        {
            get 
            {
                int h = (int)(_ScreenGraphics.DpiX * _LabelFont.SizeInPoints / (float)72 + 0.5);
                return h; 
            }
        }

        public static Graphics ScreenGraphics
        {
            get
            {
                return _ScreenGraphics;
            }
        }
        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);
    }
}
