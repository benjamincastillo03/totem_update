using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace wpf_vista_totem.modelo {
    internal class configuracion_bd  {
        //string connectionString       =   "DATA SOURCE=10.5.183.212:1521/DSSAN;USER ID=USUARIO_SISTEMAS;PASSWORD=USU2014RIOSSAN";
        //string connString             =   "DATA Source=10.68.159.13:1521/XEPDB1;User Id=admin;Password=ssprueba.210;";
        private string connStringConexion { get; set; }
        public configuracion_bd(){
            string ip_oracle            =   Environment.GetEnvironmentVariable("ip_oracle");
            this.connStringConexion     =   "Data Source="+ip_oracle+"/XEPDB1;User Id=admin;Password=ssprueba.210;";
            //Console.WriteLine("this.connStringConexion : " + this.connStringConexion);
        }

        public DataTable genera_nuevo_ticket(string ind_tipopaciente){
            //DataTable
            DataTable dt = new DataTable();
            using (OracleConnection myConnection    =   new OracleConnection()){
                //myConnection.ConnectionString     =   connString;
                myConnection.ConnectionString       =   this.connStringConexion;
                myConnection.Open();
                //execute queries
                using (OracleCommand cmd    =   new OracleCommand()){
                    cmd.Connection          =   myConnection;
                    cmd.CommandText         =   "ADMIN.HOSP_GESTION_TOTEMS.NUEVO_TICKET_FARMACIA";
                    cmd.CommandType         =   CommandType.StoredProcedure;
                    cmd.Parameters.Add("V_IND_PRIORIDAD",OracleDbType.Varchar2).Value       =   ind_tipopaciente;
                    cmd.Parameters.Add("C_OUT_TICKET",OracleDbType.RefCursor).Direction     =   ParameterDirection.Output;
                    OracleDataAdapter da    =   new OracleDataAdapter(cmd);
                    da.Fill(dt);
                    myConnection.Close();
                    return dt;
                }
            }
        }
    }
}