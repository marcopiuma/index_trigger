// See https://aka.ms/new-console-template for more information


var blobURL = "";

UVS_V1_Wrapper UVS_V1 = new UVS_V1_Wrapper("", "", "");


var result = await UVS_V1.addDocumentToIndex(blobURL, "testdocument");

Console.WriteLine(result.ToString());
