|[中文](https://github.com/WangQAQ/VRChat-Pool-table-Leaderboard-MS-VRCSA-Billiards)| |[EN](https://github.com/WangQAQ/VRChat-Pool-table-Leaderboard-MS-VRCSA-Billiards/blob/main/README-EN.md)| |[Русский](https://github.com/WangQAQ/VRChat-Pool-table-Leaderboard-MS-VRCSA-Billiards/blob/main/README-RU.md)| |[日本語](https://github.com/WangQAQ/VRChat-Pool-table-Leaderboard-MS-VRCSA-Billiards/blob/main/README-JP.md)| |[Español](https://github.com/WangQAQ/VRChat-Pool-table-Leaderboard-MS-VRCSA-Billiards/blob/main/README-ES.md)|

> Use AI Translation

# VRC Billar - Tabla de Clasificación - Snooker
## Tabla de Herencia del Proyecto
* #### MS-VRCSA-Billiards
  * #### VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards [Actualización del Hito](https://github.com/WangQAQ/VRChat-Pool-table-15-red-snooker-Pyramid-Chinese-8-MS-VRCSA-Billiards)
  	  * ### VRChat-Pool-table-Leaderboard-MS-VRCSA-Billiards (Actual)
### El propósito de esta tabla es proporcionar una experiencia entretenida, permitiendo pequeños complementos o funciones divertidas.
#### [Tabla de Derechos de Autor](https://github.com/WangQAQ/VRChat-Pool-table-Leaderboard-MS-VRCSA-Billiards/blob/main/Copyright.md)

> Puedes jugar en esta mesa aquí: [Mapa](https://vrchat.com/home/world/wrld_d9ac19bc-a8c4-42cd-b712-c66dd813bd8c/info)
---
## 1. Nuevas Funciones
* ### Panel de Carga:
	* #### Ya no es necesario vincular tareas en la función Start, los vínculos actuales se completan en EditTime, sin costo adicional de rendimiento.
	* #### Método de carga simplificado, ya no tienes que preocuparte por los vínculos, simplemente haz clic en secuencia en el panel para completar los vínculos.
	* #### Sistema de gestión de claves para el tablero de clasificación mejorado. Hemos notado que al usar tableros de clasificación automáticos, las claves a menudo se pierden debido a ciertas acciones. Ahora hemos creado un nuevo sistema de usuarios para vincular tus claves.
* ### Sistema de Usuario:
	* #### Puedes registrar tu cuenta en [Cuenta](https://www.wangqaq.com/PoolBar/Account).
	* #### Ahora puedes elegir un nombre colorido en la WEB una vez registrado.
	* #### Aunque es posible cargar claves de forma anónima, para evitar perder claves y no poder usar la tabla de clasificación en el futuro, recomendamos registrar una cuenta (la experiencia se acumulará en tu cuenta).
	* #### Ahora puedes elegir tu hito en la WEB.
* ### Nuevas Optimizaciónes:
	* #### He solucionado el problema de GC en la tabla de clasificación. Ahora, ya no habrá miles de llamadas GC.
	* #### He optimizado el sombreado para el hito, evitando el retraso.
	* #### He actualizado el sistema de nombres coloridos. Ahora, los nombres coloridos se almacenan en caché una vez creados completamente en la WEB, por lo que no habrá bucles repetidos en Udon.
  	* #### El nuevo panel para vincular scripts ya no ocupa tiempo de carga del juego en la función Start.
* ### Nuevos Beneficios:
	* #### Los nombres coloridos ahora son ilimitados. Puedes registrar tu cuenta, vincular tu jugador y elegir tu nombre colorido.
	* #### Sistema de Niveles (Nivel 6 - Nombres coloridos de 2 segmentos, Nivel 12 - Nombres coloridos de 3 segmentos, Nivel 24 - Nombres coloridos de 4 segmentos).
* ### Nueva WEB:
	* #### Se ha añadido un panel personal para configurar tu hito y nombre colorido.
	* #### Se ha añadido un panel de información personal para ver tu historial y número de cargas.
	* #### Se ha añadido una función de registro en la tabla de clasificación, ahora puedes seguir tu tasa de victorias.
	* #### Un sistema de clasificación simple basado en la tasa de victorias, lo que permite a los jugadores centrarse en más que solo en los puntos.
	* #### Más idiomas para la WEB están en desarrollo....
---

## 2. Cómo Usarlo
### Abre la carpeta Prefab (rojo es necesario, azul es opcional)
![1](https://github.com/user-attachments/assets/24566164-7c7a-4d29-b29f-d012d887821e)
* #### Coloca snooker&pyramid&cn8&3c&10b (mesa de billar principal).
* #### Coloca TableHook (réplica) 2 (complemento obligatorio).
* #### Coloca UI-Leaderboard (UI de tabla de clasificación).
* #### Coloca TagPlug (Hito principal) (complemento opcional).

## Luego encuentra VRC-VRCSA arriba.
![1](https://github.com/user-attachments/assets/09701d17-b73e-4cee-b834-ca5cb6385cdd)
* ### Primero, haz clic en "Set Up Pool Table Layers".
* ### Luego abre la Herramienta de Construcción:
	* #### Inicia sesión en tu cuenta (opcional).
	* #### Haz clic en los botones en orden de arriba a abajo.
 	* #### Esta es una versión preliminar, por lo que pueden haber errores.

## Uso del Sistema WEB (Pruebas Iniciales)
* ### En tu [Página Personal](https://www.wangqaq.com/PoolBar/Account), puedes:
	* #### Configurar tu nombre colorido.
	* #### Configurar tu hito.
	* #### Si deseas usar un nombre colorido, primero debes vincular tu usuario visitando el [mapa](https://vrchat.com/home/world/wrld_d9ac19bc-a8c4-42cd-b712-c66dd813bd8c/info) y copiando el código de usuario del Sistema de Códigos de Usuario en la WEB, luego haz clic en vincular.
 	*  ![1](https://github.com/user-attachments/assets/b2f3a365-6ebe-452e-9d75-8b798ee98ac2)
* ### En el [Tablero de Información](https://www.wangqaq.com/PoolBar/Information), puedes:
	* #### Ver tus estadísticas de temporada, nivel de usuario, lista de tareas, eventos y más.

## Plan de Actualización
	* #### Visualización histórica de rondas de billar WebGL.
 	* #### Mejora de la tabla de clasificación.
  	* #### Mini-juego abstracto de billar.
  	* #### Más idiomas para la WEB.

## Agradecimientos Especiales

### Agradecimientos especiales a todos (sin un orden específico): COCO, Arroz con huevo, Angelo Rosetta, Chuletón, Lia que come...
### Agradecimientos especiales a COCO por crear el logo, el front-end y la UI (UI y front-end se cargarán más tarde debido a limitaciones de tiempo).

## Imágenes de Alta Definición
![1](https://github.com/user-attachments/assets/22d982b4-a50e-420f-8db5-05553483445d)
![1](https://github.com/user-attachments/assets/3ab92dda-c7dc-4ab1-94dd-bce85f6809e2)
![1](https://github.com/user-attachments/assets/90a37503-a4c4-4b7f-936c-17f00c094bec)

![qrcode_1737098291587](https://github.com/user-attachments/assets/ebbfe76c-75b4-4352-b105-5e02ae20ff09)
