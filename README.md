Descargar/clonar el repositorio a una ubicación

este ejemplo se manejo a travez de visual studio code. El proyecto como tal se maneja mediante un contenedor docker con una app asp mvc.
para poder ejecutar el proyecto de forma local la carpeta raiz es donde se encuentran los archvos docker. 

Correr de forma local mediante vs code para confirmar que funciona, y posteriormente se subira a azure

El ejemplo corre sobre un app service linux. 

para el seguimiento de esta practica se debe de realizar los siguientes puntos; se recomienda utilizar todo mediante linea de comandos ya que para el examen az-204 se maneja de esta forma; aqui se emplea el Cli pero tambien se pueden realizar mediante azure powershell.

1. Creacion de un grupo de recursos en azure. 
2. creacion de un app service plan, con sku B1 y especificando que es linux :  az appservice plan create -n <PlanName> -g <GrupoRec> --sku B1 --is-linux
3. creacion de web app: az webapp create -n <appName> -g <GrupoRec> --plan <PlanName> --runtime DOTNETCORE:6.0
4. creacion de azure container registry: az acr create -g <GrupoRec> -n <acrNAme> --sku Basic.
5. habilitar usuario admin: az acr update -n <acrName> --admin-enabled true 
6. Crear una imagen de contenedor para el docker file con ACR build Task, aqui se especifican etiquetas 1.0.0 y latest: az acr build --image "asp1:1.0.0" --image "asp1:latest" --registry <acrNAme> --file "Dockerfile" .
7. obtener admin password del acr: az acr credential show -n acrxrc --query "passwords[0].value" 
8. actualizar el app service con la nueva app: az webapp config container set -g <GrupoRec> -n <appName> -r https://<acrNAme>.azurecr.io -i <acrNAme>.azurecr.io/asp1:latest -u <acrNAme> -p <password>
  
una vez seguido estos pasos deverian poder visualizarse los cambios en el app service, para esto consulten la pagina principal.

si hacen cambios en el proyecto local y desean subirlos a azure solo bastara correr los siguientes comandos:
 1. az acr build --image "asp1:1.0.0" --image "asp1:latest" --registry <acrName> --file "Dockerfile" .
 2. az webapp restart -g <GrupoRec> -n applinuxxrc                             

  
  Una vez que compilada la app el segundo paso es crear una funcion, el procedimiento se explica en los siguientes pasos. 
  
  1. Crear una function app en el protal dentro del mismo grupo de recursos.y asignarle un nombre 
      a) publicar como code.
      b) runtime .net version 6 en sistema operativo windows
      c) tipo plan consuption.
  2. Creada, entrar al recurso y crear una funcion "f(x)"
      a) seleccionar Develop in portal en enviroment
      b) seleccionar plantilla azure queue storage trigger
      c) asignar nombre en "new function" , asignar nombre a cola en "queue name" en coneccion agregar seleccion "azure web jobs storage ..."
  3. creada la funcion entrar en "code + test" y seleccionar run.csx y remplazar el codigo por defecto con el siguiente, editado guardas cambios
    
              using Microsoft.Extensions.Logging;
              public static DemoMessage Run(string myQueueItem, ILogger log)
              {
                  return new DemoMessage() {         
                      PartitionKey = "Messages",         
                      RowKey = Guid.NewGuid().ToString(),         
                      Message = myQueueItem.ToString() };
              }
              public class DemoMessage 
              {     
                  public string PartitionKey { get; set; }     
                  public string RowKey { get; set; }     
                  public string Message{ get; set; } 
              }
  
  4. entrar a integration y sobre outputs seleccionar "Add output"
      a) Binding type seleccionar "azure table storage" 
      b) account connection " azure wb jobs storage"
      c) table parameter name "$return"
      d) table name " <NombreTabla> "
  
  5. crear storage account 
  az storage account create --name <StorageAcount> --resource-group <GrupoRec>
  
  6. crear tabla 
  az storage table create --name <NombreTabla> --account-name <StorageAcount> --account-key <StorageAccountKey>
  
  7. Crear cola -- debe ser el mismo que en el punto "2" insiso "c"
  az storage queue create -n <NombreQueue> --account-key <StorageAccountKey>
  
  8. agregar al proyecto asp
     
    a) using Azure.Storage.Queues;
    b) añadir conecctions tring de storage acoount a appsetings.json
    c) agregar el metodo
            public static QueueClient CreateQueueClient()
            {
                try
                {
                    QueueClient queueClient = new QueueClient(conecctionString, queueName);
                    queueClient.CreateIfNotExists();
                    if(queueClient.Exists())
                        Console.WriteLine($"The queue created: '{queueClient.Name}'");
                    return queueClient;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}\n\n");
                    throw;
                }
            }
    d) agregar metodo 
  
        public static void InsertMessage(QueueClient queueClient, string message)
        {
            var plainText = System.Text.Encoding.UTF8.GetBytes(message);
            queueClient.SendMessageAsync(Convert.ToBase64String(plainText));
            Console.WriteLine($"Message Inserted");
        }
    e) Enviar mensaje dentro del boton guardar. 
            var client = CreateQueueClient();
            string msg = rx;
            InsertMessage(client,msg);
  
