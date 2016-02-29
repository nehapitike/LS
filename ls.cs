using System;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;


namespace LStest3
{
    public class BLAS : Hub
    {
        public string[] Blas1(String mat1, String mat2, String connID)
        {            
            var A = JsonConvert.DeserializeObject<Matrix>(mat1);  // see below for MatObject class
            var B = JsonConvert.DeserializeObject<Matrix>(mat2);  // see below for MatObject class
            int N = A.size[1]; // the number of columns in matrix A 
            int M = A.size[0]; // number of rows in matrix A
            int k = B.size[0]; // number of columns in matrix B
            if (N != M)
            {
                Clients.Client(connID).displayError1();
            }
            if (N != k)
            {
                Clients.Client(connID).displayError2(N,k);
            }
            double[,] L = new double[M, M];
            double[,] U = new double[M, M];
            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    if (j < i)
                        U[i, j] = 0;
                    else
                    {
                        U[i, j] = A.data[i, j];
                        for (int K = 0; K < i; K++)
                        {
                            U[i, j] = U[i, j] - U[K, j] * L[i, K];
                        }
                    }
                }
                for (int j = 0; j < M; j++)
                {
                    if (j < i)
                        L[j, i] = 0;
                    else if (j == i)
                        L[j, i] = 1;
                    else
                    {
                        L[j, i] = A.data[j, i] / U[i, i];              
                    }
                }
            }
            

            double[,] y = new double[M, 1];
            for (int i = 0; i < M; i++)
            {
                
                
                    y[i, 0] = 0;
                
                
            }
            //Clients.Client(connID).displayError1();
            y[0, 0] = B.data[0, 0] / L[0, 0];           
 
            for (int i = 1; i < M; i++)
            {
                double sum = 0;
                for (int z = 0; z < i; z++)
                {
                    sum = sum + L[i, z] * y[z, 0];                  
                    y[i, 0] = (B.data[i, 0] - sum) / (L[i, i]);
                    
                    
                }
            }
            
            double[,] x = new double[M, 1];
            for (int i = 0; i < M; i++)
            {              
                    x[i, 0] = 0;          

            }
            x[M-1, 0] = y[M-1, 0] / U[M-1, M-1];
            string[] D = new string[M]; // dot product D
            string strC = "";

            for (int i = M-2; i >= 0; i--)
            {
                double sum = 0;
                for (int z = M-1; z > i; z--)
                {
                    
                    sum = sum + U[i, z] * x[z, 0];
                    x[i, 0] = (y[i, 0] - sum) / U[i, i];

                }
            
            }

            for (int i = 0; i < M; i++ )
            {
                strC = x[i, 0].ToString();
                D[i] = x[i, 0].ToString();
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
            public double [,] data { get; set; }
            public int [] size { get; set; }
        }
    }
}
