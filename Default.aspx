<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
     <!-- Metas -->
    <meta charset="utf-8" />
	<meta http-equiv="X-UA-Compatible" content="IE=edge" />
	<meta name="viewport" content="width=device-width, initial-scale=1.0" />

	<!-- Title -->
	<title>Self service library | Kardan University</title>

	<!-- Bootstrap CSS -->
	<link rel="stylesheet" type="text/css" href="css/bootstrap.min.css" />

	<!-- FontAwesome Icon -->
	<link rel="stylesheet" type="text/css" href="css/font-awesome.css" />

	<!-- Odometer CSS -->
	<link rel="stylesheet" type="text/css" href="css/odometer-theme-default.css" />

	<!-- Custom CSS Styles -->
	<link rel="stylesheet" type="text/css" href="css/style.css" />

	<!-- Favicon -->
	<link rel="icon" href="img/favicon.ico" type="image/x-icon"/>
    <script src="js/jquery.min.js"></script>
    <script src="assets/bootstrap/js/bootstrap.min.js"></script>
    <link href="assets/HoldOn/HoldOn.min.css" rel="stylesheet" />
    <link href="assets/Pnotify/pnotify.custom.min.css" rel="stylesheet" />
    
 
    <style type="text/css">
    	a:hover {
    		
    		color:rgba(0, 0, 100, 0.7);
    	}
        a{
            font-size: 15px;
            color:black;
        }
    </style>
    
     <script type="text/javascript">
         function notify(title, message, type, delay) {
             new PNotify({
                 title: title,
                 text: message,
                 type: type,
                 delay: 4000
                 //hide: false


             });
         }
         
        function ResetDom() {
            document.getElementById("rfidnoTxt").value = "";
            $('#btnback').hide();
            $('#btndone').hide();
            $('#btnstart').hide();
            $("#rfidnoTxt").show();
            $("#stylel").show();
            $("#rfidnoTxt").focus();
            $("#bookcodeTxt").hide();
            $('#booksection').hide();
            $("tbody").children().remove()
            Machine("stop");

            
        }
        var myVar;
        function Machine(value) {
            clearTimeout(myVar);
             var fn = '';
             if (value == "start") {
                 fn = "StartMachine";
                 $('#btnstart').hide();
                 $('#btnstop').show();
                 myVar = setTimeout(
                   function () {
                       Machine('stop');
                   }, 54000);
             }
             else{
                 fn = "StopMachine";
                 $('#btnstart').show();
                 $('#btnstop').hide();
             }
               $.ajax({
                    type: "POST",
                    url: "Default.aspx/"+fn,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                       
                        if (response.d == "started") {
                            

                        }
                    },
                    Error: function () {}
                });
         }
        function GetStartedJS() {
            document.getElementById('bookcodeTxt').onblur = function (event) {
                var blurEl = this;
                setTimeout(function () {
                    blurEl.focus()
                }, 10);
            };
            //var rfidno = document.getElementById("rfidnoTxt").value;
            //if (rfidno.length == 10) {
                //document.getElementById("rfidnoTxt").value = "";
                document.getElementById("bookcodeTxt").value = "";
                //$('#bookcodeTxt').show();
                $('#bookcodeTxt').focus();
                $("#stylel").hide();
                $('#btnstart').show();
                $('#btnstop').hide();
                $("tbody").children().remove()
                //$('#booksection').show();
                //$('#btnback').show();
                //$('#btndone').show();
                //$('#btnstart').show();
                ////$('#rfidnoTxt').hide();
                //Machine("start");
              
           // }
           
        }
        var bcode;
        function UpdateBookJS() {
            var bookcode = $("#bookcodeTxt").val();
            if (bookcode.length == 8) {
                $.ajax({
                    type: "POST",
                    url: "Default.aspx/UpdateBook",
                    data: '{Code: "' + bookcode + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",

                    success: function (response) {
                        if (response.d == 'success') {
                            PNotify.removeAll();
                            notify('Success', 'Book Received Successfully.', 'success');
                            $("#bookcodeTxt").val('');
                            GetBookJS(bookcode);
                            bcode = bookcode;
                        }
                        else {
                           // PNotify.removeAll();
                           // notify('Warning!', response.d, 'warning');
                            $("#bookcodeTxt").val('');
                        }
                    },
                    Error: function () {
                    }
                });

                
                $("#bookcodeTxt").val('');

            }
        }
        function GetBookJS(bookcode) {
          
                $.ajax({
                    type: "POST",
                    url: "Default.aspx/GetBook",
                    data: '{Code: "' + bookcode + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (msg) {
                        var json = JSON.parse(msg.d);
                        $.each(json, function (index, obj) { 
                            var row = '<tr style="background-color:whitesmoke; font-size:16px; font-weight:500"> <td> ' + obj.BookID + ' </td> <td>' + obj.BookCode + '</td> <td>' + obj.BookName + '</td><td>' + obj.Author + '</td><td><span class="btn btn-sm btn-success">&nbsp;&nbsp; '+ obj.Type +' &nbsp;&nbsp;</span></td> </tr>'
                            $("#tbDetails tbody").append(row);
                        });
                    },
                  
                    Error: function () {
                    }
                });
        }
        function SendEmailJS() {
            if (bcode == null || bcode == "") {

            }
            else {
                
                $.ajax({
                    type: "POST",
                    url: "Default.aspx/SendEmail",
                    data: '{Code: "' + bcode + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (msg) {
                        if (msg.d == 'success') {
                            PNotify.removeAll();
                            notify('Success', 'Please check your email.', 'success');
                            

                        }
                        else {

                        }
                    },

                    Error: function () {
                    }
                });
                bcode = "";
            }
            
        }
        $(document).ready(function () {
            GetStartedJS();
        });
       
     </script>
   
    <script src="js/jquery.min.js"></script>
</head>
<body>
    	<!-- Start Preloader -->
	<div id="preloader">
		<div class="loader"></div>
	</div>
	<!-- End Preloader -->

	<!-- Start Header Section -->
    
	<section class="main-header main-2">
       
		<div class="container">
            
			<div class="row">
				<div class="col-12 col-lg-12" style="height: 40px; ">
					<div class="header-content">
                        
						<h1 class="title" data-aos="fade-down" data-aos-delay="200" style="margin-top:-80px; text-align:center">SELF SERVICE LIBRARY</h1>
						<br/>
                        <br/>
                
						<input type="password" id="bookcodeTxt" class="tapBox" onkeydown="UpdateBookJS();" placeholder="Tap here and return book" />
					</div>
				</div>

				<div class="col-12 col-lg-4" style="height: 50px">
				
				</div>
			</div>

			<div class="row">
				<div class="col-12 col-lg-12">
					
					<!-- BEGIN BOOKS TABLE -->
						<section class="blog-list">
							<div class="blog-item" >
                                <div id="stylel"> <br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /></div>
                                <div id="booksection" class='table-bordered' style='overflow: auto; height: 300px;'>

                                    <table id="tbDetails" class='booksTable'>
                                        <thead>
                                            <tr style="font-size:18px; font-weight:700">
                                              
                                                <td>Book ID</td>
                                                <td>Book Code</td>
                                                <td>Book Name</td>
                                                <td>Author</td>
                                                <td>Status</td>
                                            </tr>
                                        </thead>
                                        <tbody>
                                         
                                        </tbody>
                                    </table>
                                </div>
                               
							</div>
						</section>
					<!-- END BOOKS TABLE -->
				</div>
			
				<!-- BEGIN BUTTONS -->
				<div class="col-md-6">
                    <a class="btn btn-info" id="btnstart" href="#" onclick="Machine('start');" style="color:white"> <b> Start&nbsp;Machine</b></a>
                    <a class="btn btn-danger" id="btnstop" href="#" onclick="Machine('stop');" style="color:white"> <b> Stop&nbsp;Machine</b></a>
					<!--<a class="btn-two" id="btnback" href="#" onclick="ResetDom();" style=""><i class="fa fa-chevron-left"></i> &nbsp;&nbsp;Back</a>-->
				</div>
                <div class="col-md-2">
				
				</div>
				<div class="col-md-4" style="text-align:right">
					<a class="btn btn-success" id="btndone" href="#" onclick="GetStartedJS();SendEmailJS();" style="">&nbsp;&nbsp;<i class="fa fa-save"></i> &nbsp;&nbsp;Done&nbsp;&nbsp;</a>
				</div>
				<!-- END BUTTONS -->

			</div>
		</div>
	</section>
	<!-- End Header Section -->
		
	<!-- JQuery -->
	<script src="js/jquery.min.js"></script>

	<!-- Bootstrap -->
	<script src="js/bootstrap.min.js"></script>

	<!-- Owl Carousel -->
	<script src="js/owl.carousel.min.js"></script>

	<!-- Odometer -->
	<script src="js/odometer.min.js"></script>
    <script src="assets/Pnotify/pnotify.custom.min.js"></script>
    <script src="assets/HoldOn/HoldOn.min.js"></script>
	<!-- Custom Script -->
	<script src="js/script.js"></script>
    <script src="js/jquery.min.js"></script>
    </body>
</html>





