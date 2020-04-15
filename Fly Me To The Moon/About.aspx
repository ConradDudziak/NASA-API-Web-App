<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="Fly_Me_To_The_Moon.About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <!--Css436 Program 5 - Autumn 2019
    Conrad Dudziak, Rithik Bansal, McKinley Melton
    This Program is a website that uses the NASA api to display facts and images about space.
    This file contains the semantic structure and content of the about page.-->
    <h1>About Us</h1>
    <div id="aboutParent">
        <div id="about">Fly me to the Moon is a website where asteroid and space enthusiasts can stay up to date on the asteroids closest to home! 
            We also have a twitter account that tweets out the closest asteroid of the day! <br/> We use the <a href="https://api.nasa.gov/" target="_blank">NASA api</a> to get all our information. <br/> <br/>
            Created by: <br/>
              McKinley Melton <br/>
              Rithik Bansal <br/>
              Conrad Dudziak
        </div>
    </div>
</asp:Content>
