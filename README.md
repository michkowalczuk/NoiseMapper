# NoiseMapper
NoiseMapperApp is a simple noise mapping desktop application created for [*ArcGIS Online Spring Developers*](https://www.facebook.com/esripolska/photos/a.161362313904993/677058779002008/?type=3) competition organized by ESRI Polska Sp. z o.o. in 2014 and ranked 2nd best application. It's C# desktop application based on ArcGIS Runtime SDK for WPF.

To run you need ArcGIS Runtime SDK 10.2.3 for WPF from [here](http://resources.arcgis.com/en/communities/runtime-wpf/) and Microsft Visual Studio 2010 SP1 / 2012 / 2013.

![overall](https://user-images.githubusercontent.com/23641410/29654295-f5777180-88ac-11e7-98c8-e77cb22a7d74.PNG)

On each corner of screen there are elements with different functionality:

* In upper-right corner you can change **basemap**.

![right-upper](https://user-images.githubusercontent.com/23641410/29654894-5976c0c6-88af-11e7-8b16-38756bf0d23a.png)


* In upper-left corner there is a main panel with **Input data** / **Edit data** and **Settings** sections.

![left-upper](https://user-images.githubusercontent.com/23641410/29654889-551482ac-88af-11e7-9b9b-8ca1e11b3199.PNG)


* In lower-right corner you can modify source parameters.

![right-lower](https://user-images.githubusercontent.com/23641410/29654892-58907d6e-88af-11e7-9c19-80b0a6a32516.PNG)


* In lower-left corner there is a legend with symbology for noise level areas. You can set transparency using slider.

![left-lower](https://user-images.githubusercontent.com/23641410/29654891-5647da20-88af-11e7-8479-f96f191eac0e.PNG)


When the model is ready - added points sources with parameters and at least one calculation area you can click play and immediately  you will see results.

![results](https://user-images.githubusercontent.com/23641410/29654941-8c759b14-88af-11e7-9955-fb8f435a3c8d.PNG)


Download ZIP, run NoiseMapper.exe and have a fun! Source code in 'NoiseMapper' repository.

> :warning: Due to changes in the ArcGIS Online services, it is currently not possible to fully test the application.
