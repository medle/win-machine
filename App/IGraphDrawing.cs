﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinMachine.App
{
  public interface IGraphDrawing
  {
    void DrawAxis();
    void DrawGraph(int graphId, List<int> samples, AdcSampleProfile profile);
    void RemoveGraph(int graphId);
  }
}
