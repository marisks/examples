<%@ Control Language="c#" Inherits="Mediachase.Commerce.Manager.Order.Shipping.ShippingMethods" Codebehind="ShippingMethods.ascx.cs" %>
<%@ Register Src="~/Apps/Core/Controls/EcfListViewControl.ascx" TagName="EcfListViewControl" TagPrefix="core" %>
<core:EcfListViewControl id="MyListView" runat="server" DataKey="ShippingMethodId" AppId="Order" ViewId="ShippingMethodLanguage-List" ShowTopToolbar="true"></core:EcfListViewControl>