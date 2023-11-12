mergeInto(LibraryManager.library, {
  PostLog: function (itemCode, value)
  {
    //console.log(itemCode, value);
    if(typeof(iamSendAnalytics) != "undefined")
    {
        //let strCode = Pointer_stringify(itemCode);
        //let strValue = Pointer_stringify(value);
        //iamSendAnalytics(strCode, strValue);
        console.log("anaytics()", Pointer_stringify(itemCode), Pointer_stringify(value));
        iamSendAnalytics(Pointer_stringify(itemCode), Pointer_stringify(value));
    }
    else
    {
        console.error("iamSendAnalytics() failed");
    }
  },
});