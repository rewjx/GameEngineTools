# GameEngineTools
Some tools for unpacking, packing or converting game resources.

## Comic3
- [x] unpack CP3 and CML files. 解包CP3和CML格式文件。

## CatSystem For Unity

- [x] compress and decompress cstx script. 压缩和解压CSTX脚本。
- [x] export texts from cstx script or import texts into cstx script. 从cstx脚本导入导出文本。
- [x] Atx Image export, import(not finished)

Note:

when importing texts, command -no is not recommended. 一般不推荐用-no

## Calcite
- [x] unpack .sx files

## AQUAStyle
- [x] unpack AQUA/ASFA files, just test on Touhou Genso Wanderer -Lotus Labyrinth R-.
- [x] 支持AQUA文件头的打包替换。即只打包原封包内有的文件(即文件名相同的文件，文件内容可变。不能减少或增加文件)
- [x] 支持text.bin文本的导出和导入

## Cadath
- [x] pack cgf image. just support method 1 and method 3, method 2 is not supported. Checksum is filled with zero since the engine doesn't seem to use it.
