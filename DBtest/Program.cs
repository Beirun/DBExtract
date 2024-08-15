
using System.Data.SqlClient;

internal class Program
{
    private static void Main(string[] args)
    {
        extract();
    }

    public static void extract()
    {
        int columns = 6;
        string lines = File.ReadAllText("C:\\Users\\Beirun\\source\\repos\\DBtest\\DBtest\\table4.txt");
        string[] line = lines.Split('\n');
        List<string> result = new List<string>();
        result = line.ToList();
        List<string> output = new List<string>();
        foreach (string linee in result) if (!linee.Trim().Equals("")) output.Add(linee);

        
        List<string>[] datas = new List<string>[columns];
        for (int i = 0; i < datas.Length; i++) datas[i] = new List<string>();

        for(int i = 0; i < output.Count; i++)
        {
            output[i] = output[i].Trim();
            for (int j = 0; j < datas.Length; j++) if (i % columns == j) datas[j].Add(output[i]);
        }

        SqlConnection conn = new SqlConnection("Data Source=localhost\\SQLEXPRESS;Initial Catalog=ORILLO_DB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True");
        conn.Open();
        string comm = "";
        SqlCommand cmd = new SqlCommand();
        for (int i = 0; i < datas[0].Count; i++)
        {
            //comm = "INSERT INTO department values('" + datas[0][i] + "','" + datas[1][i] + "','" + datas[2][i] + "','" + datas[3][i] + "',NULL)";
            /*comm = "INSERT INTO employee values('" + datas[0][i] + "','" + datas[1][i] + "','" + datas[2][i] + "','" + datas[3][i] + "','" + datas[4][i] + "','" + datas[5][i] + "','" + datas[6][i] + "','" 
                + datas[7][i] + "'," + datas[8][i] + ",'" + datas[9][i] + "','" + datas[10][i] + "'," + datas[11][i] + "," + datas[12][i] + "," + datas[13][i] + ")";*/
            //comm = "INSERT INTO department values('" + datas[0][i] + "','" + datas[1][i] + "','" + datas[2][i] + "','" + datas[3][i] + "',NULL)";
            comm = "INSERT INTO emp_act values('" + datas[0][i] + "','" + datas[1][i] + "'," + datas[2][i] + "," + datas[3][i] + ",'" + datas[4][i] + "','" + datas[5][i] + "')";

            cmd = new SqlCommand(comm, conn);
            cmd.ExecuteNonQuery();
        }
        conn.Close();
    }
}