using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Media;

using WinMachine.Machine;

namespace WinMachine.App
{
  public class GraphDrawingVisual : FrameworkElement, IGraphDrawing
  {
    // Collection of child visual objects.
    private VisualCollection children;

    // Provide a required override for the VisualChildrenCount property.
    protected override int VisualChildrenCount => children.Count;

    // Provide a required override for the GetVisualChild method.
    protected override Visual GetVisualChild(int index) => children[index];

    private Size visualSize;

    private DrawingVisual axisVisual;
    private Dictionary<int, DrawingVisual> graphVisuals = new Dictionary<int, DrawingVisual>();

    public GraphDrawingVisual()
    {
      children = new VisualCollection(this);

      this.SizeChanged += delegate (object sender, SizeChangedEventArgs args) {
        visualSize = args.NewSize;
        (this as IGraphDrawing).DrawAxis();
      };
    }

    void IGraphDrawing.DrawAxis()
    {
      this.axisVisual = new GraphPainter(visualSize).DrawAxis();
      RebuildChildren();
    }

    void IGraphDrawing.DrawGraph(int graphId, List<int> samples, AdcSampleProfile profile)
    {
      var graph = new GraphPainter(visualSize).DrawGraph(
        samples, profile, ChooseColor(graphId), graphId);
      graphVisuals[graphId] = graph;
      RebuildChildren();
    }

    void IGraphDrawing.RemoveGraph(int graphId)
    {
      if (graphVisuals.ContainsKey(graphId)) {
        graphVisuals.Remove(graphId);
        RebuildChildren();
      }
    }

    private void RebuildChildren()
    {
      children.Clear();
      if (axisVisual != null) children.Add(axisVisual);
      foreach (var graphId in graphVisuals.Keys.OrderBy(x => x)) {
        children.Add(graphVisuals[graphId]);
      }
    }

    private Color ChooseColor(int graphId)
    {
      switch (graphId) { 
        case 0: return Colors.Red;
        case 1: return Colors.Green;
        case 2: return Colors.Blue;
      }
      return Colors.Black;
    }
  }
}
