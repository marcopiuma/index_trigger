// See https://aka.ms/new-console-template for more information


var blobURL = "https://marcopblob.blob.core.windows.net/videolibrary/sailboats.mp4?sp=r&st=2023-09-15T15:57:02Z&se=2023-09-15T23:57:02Z&spr=https&sv=2022-11-02&sr=b&sig=Ia5qGotM69bFTJ6dZhdYbgWVh8qJAswihihL7YsV73g%3D";

UVS_V1_Wrapper UVS_V1 = new UVS_V1_Wrapper("https://mediacopilotlibrary.cognitiveservices.azure.com/", "ee5992772ecf46be9697693bab357789", "marcoplibrary");


var result = await UVS_V1.addDocumentToIndex(blobURL, "testdocument");

Console.WriteLine(result.ToString());
