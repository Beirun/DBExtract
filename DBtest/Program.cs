
using System.Data.SqlClient;

internal class Program
{
    private static void Main(string[] args)
    {
        string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=ORILLO_DB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
        string folderPath = "C:\\Users\\Beirun\\source\\repos\\DBtest\\DBtest\\Tables";
        try
        {
            Console.Write("Select Table To Insert Records: \n1. Department\n2. Employee\n3. Project\n4. Emp_Act\n5. All\n0. End\n>>");
            int choice = Convert.ToInt32(Console.ReadLine());
            if (choice == 0) Console.WriteLine("Terminated!");
            else if (choice <0 || choice>5) Console.WriteLine("Input Error!"); 
            else if(choice == 5) for(int i = choice-2; i >=0; i--) extract(connectionString, folderPath, i);
            else extract(connectionString, folderPath, choice-1);
        }
        catch (Exception ex) { Console.WriteLine(ex); }
       
    }
    public static void extract(string connectionString, string folderPath, int choice)
    {
        string[] tables = { "Department", "Employee", "Project", "Emp_Act" };
        int[] columns = { 5, 14, 8, 6 };
        string filePath = Path.Combine(folderPath, tables[choice] + ".txt");

        string lines = File.ReadAllText(filePath); string[] line = lines.Split('\n');
        List<string> result = new List<string>();
        result = line.ToList();
        List<string> output = new List<string>();
        foreach (string linee in result) if (!linee.Trim().Equals("")) output.Add(linee);

        List<string>[] datas = new List<string>[columns[choice]];
        for (int i = 0; i < datas.Length; i++) datas[i] = new List<string>();

        for(int i = 0; i < output.Count; i++) for (int j = 0; j < datas.Length; j++) if (i % columns[choice] == j) datas[j].Add(output[i].Trim());

        SqlConnection conn = new SqlConnection(connectionString);
        conn.Open();
        SqlCommand cmd = new SqlCommand();
        for (int i = 0; i < datas[0].Count; i++)
        {
            string comm = choice switch
            {
                0 => comm = "INSERT INTO department values('" + datas[0][i] + "','" + datas[1][i] + "','" + datas[2][i] + "','" + datas[3][i] + "','" + datas[4][i] + "')",
                1 => comm = "INSERT INTO employee values('" + datas[0][i] + "','" + datas[1][i] + "'," + (datas[2][i] != "NULL" ? "'"+ datas[2][i]+"'":"NULL") + ",'" + datas[3][i] + "','" + datas[4][i] + "','" + datas[5][i] + "','" + datas[6][i] + "','" + datas[7][i] + "'," + datas[8][i] + ",'" + datas[9][i] + "','" + datas[10][i] + "'," + datas[11][i] + "," + datas[12][i] + "," + datas[13][i] + ")",
                2 => comm = "INSERT INTO project values('" + datas[0][i] + "','" + datas[1][i] + "','" + datas[2][i] + "','" + datas[3][i] + "'," + datas[4][i] + ",'" + datas[5][i] + "','" + datas[6][i] + "','" + datas[7][i] + "')",
                3 => comm = "INSERT INTO emp_act values('" + datas[0][i] + "','" + datas[1][i] + "'," + datas[2][i] + "," + datas[3][i] + ",'" + datas[4][i] + "','" + datas[5][i] + "')",
            };
            cmd = new SqlCommand(comm, conn);
            cmd.ExecuteNonQuery();
        }
        updateNulls(tables[choice], conn, cmd);

        conn.Close();
        Console.WriteLine(tables[choice] + " Done!");
    }

    public static void updateNulls(string table, SqlConnection conn, SqlCommand command)
    {
        string[] tables = { "department", "employee", "project", "emp_act" };
        int tableIndex = Array.IndexOf(tables, table.ToLower());
        string updateQuery = queryUpdate(table, conn);
        command = new SqlCommand(updateQuery, conn);
        command.ExecuteNonQuery();

    }

    private static string queryUpdate(string tableName, SqlConnection conn)
    {
        var query = @"SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName";
        using (var command = new SqlCommand(query, conn))
        {
            command.Parameters.AddWithValue("@TableName", tableName);
            using (var reader = command.ExecuteReader())
            {
                var columns = new System.Text.StringBuilder("UPDATE ").Append(tableName).Append(" SET ");
                while (reader.Read())
                {
                    var columnName = reader.GetString(0);
                    var dataType = reader.GetString(1);
                    columns.Append($"{columnName} = {(dataType == "char" || dataType == "varchar" || dataType == "text" ? $"NULLIF({columnName}, 'NULL')" : columnName)}").Append(", ");
                }
                if (columns.Length > 0) columns.Length -= 2;
                return columns.ToString();
            }
        }
    }
}