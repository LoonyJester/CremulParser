# CremulParser
This is a simple parser for CREMUL payment transaction files written in .Net. It parses the CREMUL file and creates a .C# object structure corresponding to the elements in the file.

The parser is currently not a complete parser for all kinds of CREMUl files, but is 
being developed further as needed in an ongoing project for a Norwegian customer. 

## References

Here are som useful references regarding the CREMUL file format:

1. [CREMUL documentation from bsk.no](http://bsk.no/hovedmeny/gjeldende-standarder.aspx)
2. [CREMUL documentation from truugo.com](http://www.truugo.com/edifact/d12b/cremul/)
3. [CREMUL Message Implementation Guide](http://bsk.no/media/18247/CREMUL_BSK_v2_13_d96A_Final_201204.pdf)

## Usage
```
var parser = new CremulParser();
parser.Parse("D:\\CREMUL0001.txt"));

```

## Copyright

Copyright (c) 2014 Aleksey Mykhaylichenko (alex@fortytwo.name). See LICENSE.txt for details.
