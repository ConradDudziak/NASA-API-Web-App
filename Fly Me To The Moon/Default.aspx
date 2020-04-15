<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Fly_Me_To_The_Moon._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <!--Css436 Program 5 - Autumn 2019
    Conrad Dudziak, Rithik Bansal, McKinley Melton
    This Program is a website that uses the NASA api to display facts and images about space.
    This file describes the semantic structure of the login page-->

    <div id="title">
        <h1>Welcome to <br/>Fly Me to the Moon</h1>
    </div>

    <div class="loginbox">
        <img src="images/asteroid.png" class="avatar">
        <h2>Login Here</h2>
        <div>
            <p>Username</p>
            <asp:TextBox ID="UsernameBox" runat="server" placeholder="Username"></asp:TextBox>
            <p>Password</p>
            <asp:TextBox ID="PasswordBox" runat="server" type="password" placeholder="Password"></asp:TextBox>
            <asp:Label ID="ErrorMessage" runat="server" Text="Invalid Username or Password"></asp:Label>
            <asp:Button ID="Login" runat="server" Text="Login" OnClick="Login_Click" />
            <asp:Button ID="NewUser" runat="server" Text="New User" OnClick="NewUser_Click" />
        </div>
    </div>

</asp:Content>
