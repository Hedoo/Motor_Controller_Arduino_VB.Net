Imports System.Data.SqlClient
Imports System.Data
Imports System.Web.Script.Serialization
Imports System.Net.Mail

Partial Class _Default
    Inherits System.Web.UI.Page



    Public Shared comport As New IO.Ports.SerialPort


    <System.Web.Services.WebMethod(EnableSession:=True)>
    Public Shared Function StartMachine() As String



        Try
            If comport.IsOpen Then
                comport.Write("str1")
            Else
                comport.BaudRate = 9600
                comport.PortName = "COM3"
                comport.Open()
                comport.Write("str1")

            End If

            Return "started"
        Catch ex As Exception
            comport.Close()
            Return ex.Message
        End Try
    End Function
    <System.Web.Services.WebMethod(EnableSession:=True)>
    Public Shared Function StopMachine() As String

        Try
            If comport.IsOpen Then
                comport.Write("stp1")
                comport.Close()
            End If
            Return "stopped"
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    <System.Web.Services.WebMethod(EnableSession:=True)>
    Public Shared Function UpdateBook(ByVal Code As String) As String
        Dim cn As New SqlConnection(ConfigurationManager.ConnectionStrings("conn").ToString)
        Try
            Dim cmd As New SqlCommand()



            Dim obj As Object
            cmd = New SqlCommand("SELECT BookCode FROM BooksLedger WHERE Bookcode='" & Code & "' AND (type <> 'Received' ) AND (type <> 'Returned' )", cn)

            If (cn.State = ConnectionState.Closed) Then
                cn.Open()
            End If
            obj = cmd.ExecuteScalar()
            If IsNothing(obj) = False Then
                cmd = New SqlCommand("UPDATE BooksLedger SET Date=@date,Type=@type, Received=1, Issued=0 WHERE Issued=1 AND EmployeeID=0 AND BookCode=@BookCode", cn)
                cmd.Parameters.AddWithValue("@BookCode", Code)
                cmd.Parameters.AddWithValue("@date", Now.Date)
                cmd.Parameters.AddWithValue("@type", "Returned")
                If (cn.State = ConnectionState.Closed) Then
                    cn.Open()
                End If
                cmd.ExecuteNonQuery()




                Return "success"
            End If
            cn.Close()

            Return "returned"
        Catch ex As Exception
            cn.Close()
            Return ex.Message
        End Try
    End Function

    <System.Web.Services.WebMethod(EnableSession:=True)>
    Public Shared Function GetBook(ByVal Code As String) As String
        Dim cn As New SqlConnection(ConfigurationManager.ConnectionStrings("conn").ToString)
        Try
            Dim cmd As New SqlCommand()
            Dim dr1 As SqlDataReader
            Dim str1 As String = ""
            Dim i As Decimal = 1
            Dim da As SqlDataAdapter
            Dim dt As DataTable
            cmd = New SqlCommand("SELECT Top 1 Book.BookID,BooksLedger.BookCode,Book.BookName,Book.Author,BooksLedger.Type FROM Book " &
            "INNER JOIN BooksLedger ON BooksLedger.BookID=Book.BookID WHERE Bookcode='" & Code & "' AND BooksLedger.Type='Returned'", cn)

            If (cn.State = ConnectionState.Closed) Then
                cn.Open()
            End If
            dr1 = cmd.ExecuteReader()
            'str1 = "<table class='booksTable'><thead><tr style='font-size:18px; font-weight:700'><td>#</td><td>Book ID</td><td>Book Code</td><td>Book Name</td><td>Author</td><td>Status</td></tr></thead><tbody>"
            While (dr1.Read())
                str1 += "<tr style='background-color:whitesmoke; font-size:16px; font-weight:500'><td>" & i & "</td><td>" & dr1.Item("BookID") &
                "</td><td>" & dr1.Item("BookCode") & "</td><td>" & dr1.Item("BookName") & "</td><td>" & dr1.Item("Author") & "</td><td><span class='btn btn-sm btn-success'>&nbsp;&nbsp;" & dr1.Item("Type") & "&nbsp;&nbsp;</span></td></tr>"

                i = i + 1
            End While
            'str1 += "</tbody></table>"
            dr1.Close()

            da = New SqlDataAdapter("SELECT top 1 ROW_NUMBER() OVER(ORDER BY Book.BookID) AS SNo, Book.BookID,BooksLedger.BookCode,Book.BookName,Book.Author,BooksLedger.Type FROM Book " &
            "INNER JOIN BooksLedger ON BooksLedger.BookID=Book.BookID WHERE Bookcode='" & Code & "' AND BooksLedger.Type='Returned'", cn)
            dt = New DataTable
            da.Fill(dt)
            cn.Close()


            Dim str As String = String.Empty
            Dim serializer As JavaScriptSerializer = New JavaScriptSerializer()
            Dim rows As List(Of Dictionary(Of String, Object)) = New List(Of Dictionary(Of String, Object))()
            Dim row As Dictionary(Of String, Object)

            For Each dr As DataRow In dt.Rows
                row = New Dictionary(Of String, Object)()

                For Each col As DataColumn In dt.Columns
                    row.Add(col.ColumnName, dr(col))
                Next

                rows.Add(row)
            Next

            str = serializer.Serialize(rows)
            Return str

        Catch ex As Exception
            cn.Close()
            Return ex.Message
        End Try
    End Function
    <System.Web.Services.WebMethod(EnableSession:=True)>
    Public Shared Function SendEmail(ByVal Code As String) As String
        Dim cn As New SqlConnection(ConfigurationManager.ConnectionStrings("conn").ToString)

        Try
            Dim cmd As SqlCommand
            ''  EMAIL SENDING CODE STARTED 
            Dim mm As New MailMessage()
            Dim Message As String
            Dim studentinfo As SqlDataReader
            Dim pno As Object
            cmd = New SqlCommand("SELECT TOP 1 pno FROM BooksLedger WHERE Bookcode='" & Code & "' AND Type='Returned' ORDER BY ID DESC ", cn)
            If (cn.State = ConnectionState.Closed) Then
                cn.Open()
            End If
            pno = cmd.ExecuteScalar()
            If pno = "" Then
                cmd = New SqlCommand("SELECT Top 1 Employee FROM BooksLedger WHERE Bookcode='" & Code & "' AND Type='Returned' ORDER BY ID DESC ", cn)
                If (cn.State = ConnectionState.Closed) Then
                    cn.Open()
                End If
                Dim EmployeeID As Object
                EmployeeID = cmd.ExecuteScalar()
                cmd = New SqlCommand("SELECT EmployeeName AS StudentName,Email FROM Employee WHERE EmployeeID=" & EmployeeID, cn)
                If (cn.State = ConnectionState.Closed) Then
                    cn.Open()
                End If
                studentinfo = cmd.ExecuteReader()

            Else
                cmd = New SqlCommand("SELECT FirstName + ' ' + MiddleName + ' ' + LastName AS StudentName,Email FROM Student WHERE pno='" & pno & "'", cn)
                If (cn.State = ConnectionState.Closed) Then
                    cn.Open()
                End If
                studentinfo = cmd.ExecuteReader()

            End If


            While studentinfo.Read()
                mm.From = New MailAddress("noreply@email.kardan.edu.af")
                mm.Bcc.Add(New MailAddress(studentinfo.Item("Email")))
                mm.Subject = "Self Service Library | Kardan Univeristy"
                mm.IsBodyHtml = True





                Dim bookinfo As SqlDataReader
                If pno = "" Then
                    cmd = New SqlCommand("SELECT Employee FROM BooksLedger WHERE Bookcode='" & Code & "' AND Type='Returned' ", cn)
                    If (cn.State = ConnectionState.Closed) Then
                        cn.Open()
                    End If
                    Dim EmployeeID As Object
                    EmployeeID = cmd.ExecuteScalar()
                    cmd = New SqlCommand("SELECT top 1 BookName,Author,Edition,DATEADD(DAY,10,Date) AS Date FROM Book INNER JOIN BooksLedger ON BooksLedger.BookID=Book.BookID WHERE Employee = @employeeid AND Type = 'Returned' AND Date = @date", cn)
                    cmd.Parameters.AddWithValue("@employeeid", EmployeeID)
                    cmd.Parameters.AddWithValue("@date", Now.Date)
                    bookinfo = cmd.ExecuteReader()
                Else

                    cmd = New SqlCommand("SELECT top 1 BookName,Author,Edition,DATEADD(DAY,10,Date) AS Date FROM Book INNER JOIN BooksLedger ON BooksLedger.BookID=Book.BookID WHERE pno = @pno AND Type = 'Returned' AND Date = @date", cn)
                    cmd.Parameters.AddWithValue("@pno", pno)
                    cmd.Parameters.AddWithValue("@date", Now.Date)
                    bookinfo = cmd.ExecuteReader()
                End If
                Message &= "Dear  <b> " & studentinfo.Item("StudentName") & " </b>:<br/><br/><br/> The following book(s) have been successfully returned to library. <br/><br/>"
                While bookinfo.Read()

                    Message &= "<table><tr><td>Book Name</td><td><b>" & bookinfo.Item("BookName") &
                        "</b></td></tr><tr><td>Author</td><td><b>" & bookinfo.Item("Author") & "</b></td></tr><tr><td>Edition</td><td><b> " &
                        bookinfo.Item("Edition") & "</b></td></tr><tr><td>Date of return</td><td><b>" & Format(Now, "dd/MMM/yy") &
                        "</b></td></tr></table><br><br>"
                End While

                Message &= "<br><br>Thank you for using Kardan University Library services.<br> <br> Best, <br><br> The Librarian <br><br> Kardan University Library"

                mm.Body = mm.Body & Message
            End While
            mm.Priority = MailPriority.High
            Dim sendr As New SmtpClient("email.kardan.edu.af", 25)
            sendr.Credentials = New System.Net.NetworkCredential("noreply@email.kardan.edu.af", "XSO9v=z")
            sendr.Send(mm)
            '' EMAIL SENDING CODE ENDED
            cn.Close()
            Return "success"
        Catch ex As Exception
            cn.Close()
            Return ex.Message
        End Try
    End Function
End Class
