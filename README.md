# civil3d_2_ifc
Plugin to convert drawing's objects to IFC (with GeometryGym .NET library)
## External dependencies
- GeometryGym .NET Library (use as Nuget-package or by [that repository](https://github.com/GeometryGym/GeometryGymIFC));
- internal AutoCAD's and Civil3D's dynamic libraries: accoremgd, acdbmgd, acmgd, AecBaseMgd, AeccDbMgd, AeccPressurePipesMgd, AecPropDataMgd;

## Supported IFC formats and objects
### Ifc version
- IFC4x1;
### Civil3d objects
- Surfaces;
### Internal drawing parameters
- Property sets;

## Using (en)
There are two variants: download repository and compiling that or downloading latest release as ZIP archive (civil2ifc_ver-\*\*\*.zip). Load library to Civil3D via command NETLOAD (file "civil2ifc.dll"). Plugin contains one command: **\_ifc_export** (export to IFC current drawing's objects). Result file will be saved near that DWG.

## Использование (ru)
2 варианта: загрузка репозитория и сборка или скачивание скомпилированной версии последнего релиза (архив с плагином (civil2ifc_ver-\*\*\*.zip)) и последующая загрузка в Civil3D библиотеки "civil2ifc.dll" через команду NETLOAD). Используйте команду **\_ifc_export** для экспорта объектов Civil3D из текущего чертежа. Файл сохранитя рядом с текущим файлом.
