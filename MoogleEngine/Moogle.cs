namespace MoogleEngine;
using System.IO;
using System;
public class Corpus{
    Corpus Este = new Corpus(); 
    string FilePath = @"C:\Skewl\Proyecto Prog\moogle-main\Content";
    string[] Titles; //los titulos de los documentos
    double[,] TfIDFMatrix; //Matriz con valores de TFIDF de cada palabra en cada documento
    public string[] DocPaths; //Path de cada documento
    string[] WordVector; //vector con todas las palabras del corpus con su valor de IDF asignado

    public Corpus()
    {
        TfIDFMatrix = TF_IDF(TFMatrix(FilePath),IDFVector(FilePath));
        DocPaths = Directory.GetFiles(FilePath) ;
        WordVector = AllWords(FilePath);
        Titles = GetTitulos(FilePath);
    }
    public string[] CloneTitles()
    {
        string[] output = Titles;
        return output;
    }
    public string[] CloneAllWords()
    {
        string[] output = AllWords(FilePath);
        return output;
    }
    public double[] CloneIDFVector()
    {
        double[] output = IDFVector(FilePath);
        return output;
    }
    public double[,] CloneMatrix()
    {
        double[,] output = TfIDFMatrix;
        return output;
    }
    private string[] GetTitulos(string path)
    {
        string[] Paths = Directory.GetFiles(path);
        string[] Titulos = new string[Paths.Length];
        for (int i = 0; i < Paths.Length; i++)
        {
            Titulos[i] = Path.GetFileNameWithoutExtension(Paths[i]);
        }
        return Titulos;
    }
    private double[,] TF_IDF(int[,] TF, double[] IDF)
    {
        double[,] output = new double[TF.GetLength(0),TF.GetLength(1)] ;
        for (int i = 0; i < TF.GetLength(0); i++)
        {
            for (int j = 0; j < IDF.Length; j++)
            {
                output[i,j] = TF[i,j]*IDF[j];
            }
        }
        return output;
    }
    private double[] IDFVector(string Path)
    {
        int[,] DocMatrix = TFMatrix(FilePath);
        int N = Directory.GetFiles(Path).Length; // cantidad total de documentos
        int[] n = new int[DocMatrix.GetLength(0)]; // cantidad de documentos donde aparece la palabra
        
        for (int i = 0; i < n.Length; i++)
        {
            for (int j = 0; j < DocMatrix.GetLength(1); j++)
            {
                if(DocMatrix[i,j] != 0)
                {
                    n[i] ++;
                }
            }
        }
        double [] output = new double[n.Length];
        for (int i = 0; i < n.Length; i++)
        {
            output[i] = Math.Log10( N / n[i]) ;
        }
        return output;
    }
    private int[,] TFMatrix(string Path)
    {
        string[] Todas_Palabras = AllWords(Path);
        string[] Documents = Directory.GetFiles(Path);
        int[,] OutputMatrix = new int[Todas_Palabras.Length , Documents.Length]; // documentos en filas y palabras en columnas

        for(int i = 0; i<Documents.Length; i++)
        {
            for(int j = 0 ; j < Todas_Palabras.Length ; j++)
            {
                OutputMatrix[j,i] = Count(Todas_Palabras[j] , Documents[i]);
            }
        }
        return OutputMatrix;
    }
    private int Count(string word , string documentPath)
    {
        int repeticiones = 0;
        string [] Document = SplitText(File.ReadAllText(documentPath)); //array de todas las palabras del documento (repetidas)
        for(int i = 0 ; i < Document.Length ; i ++)
        {
            if( Document[i] == word)
            {
                repeticiones ++;
            }
        }
        return repeticiones;
    }    
    public static string[] SplitText(string texto) //convierte un texto en un array de palabras
    {
        char[] delimiters = new char[] {' ',',','.',';',':','-','_','(',')','[',']','{','}','/','\n','\r','\t'};
        string[] split = texto.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        return split;
    }
    private string[] AllWords(string Path) //creando un array con todas las palabras del Corpus
    {
        HashSet<string> words = new HashSet<string>();
        
        foreach(string FilePath in Directory.GetFiles(Path)) //obtiene los paths con el GetFiles  
        {
           string[] SplitContent = SplitText(File.ReadAllText(FilePath));   //lee cada uno con ReadAllText

            foreach(string splitword in SplitContent)
            {
                words.Add(splitword.ToLower());
            }
        }
        return words.ToArray();
    }
public double[] Vectorize(string query)
    {
        double[] IDF = IDFVector(FilePath);
        string[] SplitQuery = SplitText(query);
        string[] Words =AllWords(FilePath);
        double[] output = new double[Words.Length];
        for (int i = 0; i < SplitQuery.Length; i++)
        {
            for (int j = 0; j < Words.Length; j++)
            {
                if(Words[j] == SplitQuery[i])
                {
                    output[j] = IDF[j];
                }
                else
                {
                    output[j] = 0;
                }
            }
        }
        return output;
    }
public double[] VectorProduct(double[] queryVector)
{
    double[,] Matrix = TfIDFMatrix;
    double[] output = new double[Matrix.GetLength(0)];
    for (int i = 0; i < Matrix.GetLength(0); i++)
    {
        for (int j = 0; j < queryVector.Length; j++)
        {
            if(queryVector[j] == 0)
            {
                continue;
            }
            output[i] += Matrix[i,j]*queryVector[j];
        }
    }
    return output;
}
}
public class Moogle
{
        Corpus Este = new Corpus(); 

    public string PrimeraOracionQueContiene(string texto , string palabra) //Halla la primera oracion de un texto que contenga la palabra introducida
    {
        string[] oraciones = texto.Split(".");
        foreach (string oracion in oraciones)
        {
            if(oracion.Contains(palabra))
            {
                return oracion;
            }
        }
        return null;
    }
     public static int LastValueIndex(double[] array) // devuelve el indice del ultimo valor antes de 0 en un array ordenado (-1 si esta vacio)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if(array[i] == 0)
            {
                return i-1;
            }
        }
        return 0;
    }
     public static void OrdenandoArrays(double[] values, string[] titulos, string[] snippets)
    {
        List<Tuple<double, string, string>> listaTuplas = new List<Tuple<double, string, string>>();

        for (int i = 0; i < values.Length; i++)
        {
            listaTuplas.Add(new Tuple<double, string, string>(values[i], titulos[i], snippets[i]));
        }


        listaTuplas.Sort(); // Ordenar la lista de tuplas en función del valor del primer array


        for (int i = 0; i < listaTuplas.Count; i++)// Extraer los valores de los otros dos arrays en el orden correcto
        {
            values[i] = listaTuplas[i].Item1;
            titulos[i] = listaTuplas[i].Item2;
            snippets[i] = listaTuplas[i].Item3;
        }
    }
    public static string PalabraMasImportante(string query)
    {
        double[] IDFs = Este.CloneIDFVector;
        string[] Words = Este.CloneAllWords;
        string[] splitquery = Corpus.SplitText(query);
        int[] indexes = new int[splitquery.Length];
        
        for (int i = 0; i < indexes.Length; i++)
        {
            for (int j = 0; j < Words.Length; j++)
            {
                if(Words[j] == splitquery[i])
                {
                    indexes[i] = j;
                    break;
                }
            }
            
        }
        double first = IDFs[0];
        int index = 0;
        for (int i = 1; i < indexes.Length; i++)
        {
            
            if(IDFs[indexes[i]] < first)
            {
                first = IDFs[indexes[i]];
                index = indexes[i];
            }
        }
        return Words[index];
    }

public string[] CreateSnippets(string query)
    {
        string[] Textos = new string[Este.DocPaths.Length];
        string[] output = new string[Textos.Length];
        for (int i = 0; i < Este.DocPaths.Length; i++)
        {
            string path = Este.DocPaths[i];
            Textos [i] = File.ReadAllText(path);
        }
        string[] FirstOraciones = new string[Textos.Length];
        for (int i = 0; i < Textos.Length; i++)
        {
           FirstOraciones[i] = Textos[i].Split(".")[0] ;
        }
        string[] OracionQuery = new string[Textos.Length];
        for (int i = 0; i < Textos.Length; i++)
        {
            OracionQuery[i] = PrimeraOracionQueContiene(Textos[i] , query);
        }
        for (int i = 0; i < Textos.Length; i++)
        {
            if(FirstOraciones[i] == OracionQuery[i])
            {
            output[i] = FirstOraciones[i];
            }
            output[i] = FirstOraciones[i] + " (...) " + OracionQuery[i];
        }
        return output;
    }
    public static SearchResult Query(string query) {
        // Modifique este método para responder a la búsqueda

        double[] QueryVector = Este.Vectorize(query); // convirtiendo el query en un vector segun el corpus inicializado
        //Construyendo elementos del SearchItem
        double[] VectorProduct = Este.VectorProduct(QueryVector); // Valores de la similitud coseno entre la query y cada documento
        string[] titulos = Este.CloneTitles(); // Titulos de cada documento
        string[] snippets = Este.CreateSnippets(PalabraMasImportante(query)); //Snippets 
        //Ordenamos los resultados
        OrdenandoArrays(VectorProduct , titulos , snippets);
        //Para descartar documentos que no se relacionen con la busqueda buscamos el indice del ultimo valor diferente de 0
        int items_index = LastValueIndex(VectorProduct);

        SearchItem[] items = new SearchItem[items_index];
        
        for (int i = 0; i < items_index; i++)
            {
                items[i] = new SearchItem(titulos[i] , snippets[i] , ((float)VectorProduct[i]));
            }
        

        return new SearchResult(items, query);
    }
}
