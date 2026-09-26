using System.Collections.Generic;

namespace Projekat2
{
    static class GallagerB
    {
        public static int[] Decode(int[,] H, int m, int n, int[] received, double th0, double th1, int maxIterations, out bool success)
        {
            var checkNeighbors = new List<int>[m];
            var varNeighbors = new List<int>[n];

            for (int c = 0; c < m; c++)
            {
                checkNeighbors[c] = new List<int>();
            }
            for (int v = 0; v < n; v++)
            {
                varNeighbors[v] = new List<int>();
            }

            for (int c = 0; c < m; c++)
            {
                for (int v = 0; v < n; v++)
                {
                    if (H[c, v] == 1)
                    {
                        checkNeighbors[c].Add(v);
                        varNeighbors[v].Add(c);
                    }
                }
            }

            var varToCheck = new Dictionary<(int v, int c), int>();
            for (int v = 0; v < n; v++)
            {
                foreach (int c in varNeighbors[v])
                {
                    varToCheck[(v, c)] = received[v];
                }
            }

            int[] decision = (int[])received.Clone();

            for (int iter = 0; iter < maxIterations; iter++)
            {
                var checkToVar = new Dictionary<(int c, int v), int>();
                for (int c = 0; c < m; c++)
                {
                    var neigh = checkNeighbors[c];
                    int count = neigh.Count;

                    for (int idx = 0; idx < count; idx++)
                    {
                        int v = neigh[idx];
                        int parityExcluding = 0;

                        for (int idx2 = 0; idx2 < count; idx2++)
                        {
                            if (idx2 == idx)
                            {
                                continue;
                            }
                            parityExcluding ^= varToCheck[(neigh[idx2], c)];
                        }

                        checkToVar[(c, v)] = parityExcluding;
                    }
                }

                var newVarToCheck = new Dictionary<(int v, int c), int>();
                for (int v = 0; v < n; v++)
                {
                    int cur = received[v];
                    int degree = varNeighbors[v].Count;

                    foreach (int cTarget in varNeighbors[v])
                    {
                        int disagree = 0;
                        foreach (int c in varNeighbors[v])
                        {
                            if (c == cTarget)
                            {
                                continue;
                            }
                            if (checkToVar[(c, v)] != cur)
                            {
                                disagree++;
                            }
                        }

                        double threshold = (cur == 0 ? th0 : th1) * (degree - 1);
                        newVarToCheck[(v, cTarget)] = disagree > threshold ? 1 - cur : cur;
                    }
                }
                varToCheck = newVarToCheck;

                for (int v = 0; v < n; v++)
                {
                    int cur = received[v];
                    int degree = varNeighbors[v].Count;
                    int disagree = 0;

                    foreach (int c in varNeighbors[v])
                    {
                        if (checkToVar[(c, v)] != cur)
                        {
                            disagree++;
                        }
                    }

                    double threshold = (cur == 0 ? th0 : th1) * degree;
                    decision[v] = disagree > threshold ? 1 - cur : cur;
                }

                if (AllChecksSatisfied(checkNeighbors, m, decision))
                {
                    success = true;
                    return decision;
                }
            }

            success = AllChecksSatisfied(checkNeighbors, m, decision);
            return decision;
        }

        private static bool AllChecksSatisfied(List<int>[] checkNeighbors, int m, int[] x)
        {
            for (int c = 0; c < m; c++)
            {
                int parity = 0;
                foreach (int v in checkNeighbors[c])
                {
                    parity ^= x[v];
                }
                if (parity != 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}