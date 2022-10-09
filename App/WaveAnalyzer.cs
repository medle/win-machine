using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinMachine.App
{
    public class WaveAnalyzer
    {
        public enum SearchState { Initial, Below, Above };

        private SearchState searchState;
        private int centerSample = 0;
        private int centerHz = 0;
        private int belowSample = 0;
        private int aboveSample = 0;
        private int stepHz = 10;

        public void Reset()
        {
            searchState = SearchState.Initial;
        }

        public int Analyze(int hz, List<int> samples) 
        {
            int maxSample = samples.Max();

            switch (searchState) {

                case SearchState.Initial:
                    centerSample = maxSample;
                    centerHz = hz;
                    searchState = SearchState.Below;
                    return centerHz - stepHz;

                case SearchState.Below:
                    belowSample = maxSample;
                    searchState = SearchState.Above;
                    return centerHz + stepHz;

                case SearchState.Above:
                    aboveSample = maxSample;
                    searchState = SearchState.Initial;
                    if (centerSample >= belowSample && centerSample >= aboveSample) return centerHz;
                    if (belowSample >= centerSample && belowSample >= aboveSample) return centerHz - stepHz;
                    if (aboveSample >= centerSample && aboveSample >= belowSample) return centerHz + stepHz;
                    break;
            }

            return hz;
        }
    }
}
