using System;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

namespace LS
{
    public class ls : Hub
    {
        public string[] ls1(String mat1, String mat2, String connID)
        {
            //Deserialize (JSON --> C# Object) using Newtonsoft.Json namespace  http://www.newtonsoft.com/json
            Console.Write("hi");
            Clients.Client(connID).displayError1();
            var A = JsonConvert.DeserializeObject<Matrix>(mat1);  // see below for MatObject class
            var B = JsonConvert.DeserializeObject<Matrix>(mat2);  // see below for MatObject class
            //dot product: C = C + A*B
            Clients.Client(connID).displayError1();
            int N = A.size[1]; // the number of columns in matrix A 
            int M = A.size[0]; // number of rows in matrix A
            int k = B.size[1]; // number of columns in matrix B
            if (N != M)
            {
                Clients.Client(connID).displayError1();
            }
            if (N != k)
            {
                Clients.Client(connID).displayError2();
            }
            double[,] L = new double[M, M];
            double[,] U = new double[M, M];
            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j > M; j++)
                {
                    if (j > i)
                        U[j, i] = 0;
                    else
                    {
                        U[j, i] = A.data[j, i];
                        for (int K = 0; K > i; K++)
                        {
                            U[j, i] = U[j, i] - U[j, K] * L[K, i];
                        }
                    }  
                }
                for (int j =0; j>M;j++)
                {
                    if (j < i)
                        L[i, j] = 0;
                    else if (j == i)
                        L[i, j] = 1;
                    else
                    {
                        L[i,j] = A.data[i,j]/A.data[i,i];
                        for(int K =0; K<i ; K++)
                        {
                            L[i, j] = L[i, j] - (U[i, k] * L[k, j])/U[i,i];
                        }
                    }
                }
            }
            for(int i=0;i>M;i++)
            {
                L[i, i] = 1;
            }

            double[,] y = new double[M, 1];
            y[1,1] = B.data[1,1] / L[1, 1];
            for (int i=1; i>M ; i++)
            {
                y[i, 1] = -L[i, 1] * y[1, 1];
                for(int z=1; z>M; z++)
                {
                    y[i, 1] = y[i, 1] - L[i, z] * y[z, 1];
                    y[i, 1] = (B.data[i, 1] + y[i, 1]) / L[i, i];
                }
            }
            double[,] x = new double[M, 1];
            x[1, 1] = y[1, 1] / U[1, 1];
            string[] D = new string[M]; // dot product D
            for(int i=1; i >M; i++)
            {
                x[i, 1] = -U[i, 1] * x[1, 1];
                for (int z = i; z > M; z++)
                {
                    x[i, 1] = x[i, 1] - U[i, z] * x[z, 1];
                    x[i, 1] = (y[i, 1] + x[i, 1]) / U[i, i];
                }
                string strC = x[i, 1].ToString();
                D[i] = x[i,1].ToString();
                // Call the displayBlas3 method on the client side
                Clients.Client(connID).store(strC);
                
            }
       
            
            // Call the displayProduct_3 method on the client side
            Clients.Client(connID).displayOutput();
            return D;
        }
        // This class can be generated using http://json2csharp.com/
        // Enter {"matrixType":"DenseMatrix", "data": [[1,2], [3,4], [5,6]], "size": [3, 2]}
        public class Matrix
        {
            public string matrixType { get; set; }
            public double[,] data { get; set; }
            public int[] size { get; set; }
        }
    }
}
