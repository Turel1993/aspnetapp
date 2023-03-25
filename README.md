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
