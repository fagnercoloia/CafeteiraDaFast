<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CafeteiraDaFast.ViewModels.IndexViewModel>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Cafeteira da Fast
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            var loadMensagem = function () {
                $.ajax({
                    dataType: "json",
                    url: '<%= Url.Action("GetStatus") %>',
                    success: function (data) {
                        $("#messagem").html(data.Mensagem);
                    }
                });
            };
            setTimeout(loadMensagem, 30000);
        });
    </script>
    <div style="color: Red">
        <%= Model.MensagemErro %></div>
    <div id="messagem" class="message">
        <%=Model.Mensagem %>
    </div>
    <br />
    <%if (Model.BotaoIniciarVisivel)
      { %>
    <div style="margin-bottom: 10px;">
        <a class="botao" id="btnIniciando" href="<%= Url.Action("Iniciar") %>">Iniciar</a>
    </div>
    <%}
      if (Model.BotaoProntoVisivel)
      { %>
    <div>
        <a class="botao" id="btnPronto" href="<%= Url.Action("Pronto") %>">Pronto!!!</a>
    </div>
    <%} %>
    <br />
    <asp:HyperLink ID="HyperLink1" runat="server" 
        NavigateUrl="~/Content/CafeteiraDaFast.apk">CafeteiraDaFast.apk</asp:HyperLink>
</asp:Content>
