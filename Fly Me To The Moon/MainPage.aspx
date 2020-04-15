<%@ Page Title="Contact" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MainPage.aspx.cs" Inherits="Fly_Me_To_The_Moon.Contact" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <!--Css436 Program 5 - Autumn 2019
    Conrad Dudziak, Rithik Bansal, McKinley Melton
    This Program is a website that uses the NASA api to display facts and images about space.
    This file contains the semantic structure and content of the main page which displays space images and tables of data.-->
    <h1>Fly Me to the Moon</h1>

    <div id="POTD">
        <h2 class="section">Picture of the Day</h2>
        <p>
            <asp:Image ID="SpacePictureDisplay" runat="server" />
        </p>
    </div>

    <div id="Asteroids">
        <h2 class="section">Asteroid facts of the Day</h2>
        <asp:Table id="Table1" 
            GridLines="Both" 
            HorizontalAlign="Center" 
            Font-Names="Verdana" 
            Font-Size="12pt" 
            CellPadding="20" 
            CellSpacing="0" 
            Runat="server"/>

        <div id="Twitter">
            <h2 class="section" style="color: black; padding: 5px 0 0 0">Twitter Bot</h2>
            <p>
                <img id="twitterImage" src="images/twitter.jpg"/>
                Follow us on twitter to get the latest asteroid facts of the day!
                <a href="https://twitter.com/436Css" target="_blank">@CSS436_Closest_Asteroid</a>
            </p>
        </div>
    </div>
</asp:Content>
