
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace WinMachine.App
{
    public class GraphPainter
    {
        private Size visualSize;

        private const int padWidth = 10;

        private double graphX => padWidth;
        private double graphY => visualSize.Height - padWidth;
        private double graphW => visualSize.Width - (padWidth * 2);
        private double graphH => visualSize.Height - (padWidth * 2);

        public GraphPainter(Size visualSize)
        {
            this.visualSize = visualSize;
        }

        public DrawingVisual DrawAxis()
        {
            var drawingVisual = new DrawingVisual();
            DrawingContext dc = drawingVisual.RenderOpen();

            var axisPen = new Pen(new SolidColorBrush(Colors.Silver), 2);
            var divPen = new Pen(new SolidColorBrush(Colors.Silver), 1);

            int numDivs = 10;

            // vertical divisor lines
            double divW = graphW / numDivs;
            for (int i = 0; i <= numDivs; i++)
            {
                double divX = graphX + divW * i;
                Pen pen = (i == 0 || i == numDivs || i == numDivs / 2) ? axisPen : divPen;
                dc.DrawLine(pen, new Point(divX, graphY), new Point(divX, graphY - graphH));
            }

            // horizontal divisor lines
            double divH = graphH / numDivs;
            for (int i = 0; i <= numDivs; i++)
            {
                double divY = graphY - divH * i;
                Pen pen = (i == 0 || i == numDivs || i == numDivs / 2) ? axisPen : divPen;
                dc.DrawLine(pen, new Point(graphX, divY), new Point(graphX + graphW, divY));
            }

            dc.Close();
            return drawingVisual;
        }

        private void DrawGraphLegend(
            DrawingContext dc, List<int> samples, AdcSampleProfile profile, Color color, int graphId)
        {
            var graphBrush = new SolidColorBrush(color);
            int maxInput = samples.Max();
            double maxOutput = profile.TranslateToOutput(maxInput);

            string text = 
                $"{graphId}: ({profile.MinOutput},{profile.MaxOutput})" +
                $" MAX={maxOutput:f2}{profile.OutputUnit}";

            var typeface = new Typeface("Arial");
            double emSize = (graphH / 20);
            double pixelsPerDip = 1;

            var ft = new FormattedText(text, CultureInfo.CurrentCulture,
              FlowDirection.LeftToRight, typeface, emSize, graphBrush, pixelsPerDip);
            dc.DrawText(ft, new Point(graphX, graphY - (ft.Height * graphId) - ft.Height));
        }

        public DrawingVisual DrawGraph(
            List<int> samples, AdcSampleProfile profile, Color color, int graphId)
        {
            var drawingVisual = new DrawingVisual();
            DrawingContext dc = drawingVisual.RenderOpen();

            var graphBrush = new SolidColorBrush(color);
            var graphPen = new Pen(graphBrush, 2);

            DrawGraphLegend(dc, samples, profile, color, graphId);

            int numPoints = samples.Count();
            if (numPoints > 1)
            {
                double pointX = graphX;

                // number of line segments is number of points - 1
                double pointStep = graphW / (numPoints - 1);

                var lastPoint = new Point();
                foreach (var sample in samples)
                {
                    double pointH = profile.TranslateToRate(sample) * graphH;
                    double pointY = graphY - pointH;
                    var point = new Point(pointX, pointY);

                    double pointRadius = graphPen.Thickness / 2;
                    dc.DrawEllipse(graphBrush, graphPen, point, pointRadius, pointRadius);

                    if (pointX > graphX)
                    {
                        dc.DrawLine(graphPen, lastPoint, point);
                    }

                    lastPoint = point;
                    pointX += pointStep;
                }
            }

            dc.Close();
            return drawingVisual;
        }
    }
}
