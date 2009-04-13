ASSEMBLY = ../bin/Fmacj.Examples.Mandelbrot.exe

FILES =  \
	Main.cs \
	Mandelbrot.cs \
	Properties/AssemblyInfo.cs

REFERENCES =  \
	-r:../bin/Fmacj.Emitter.dll \
	-r:../bin/Fmacj.Framework.dll \
	-r:System.Drawing

all: $(ASSEMBLY)

$(ASSEMBLY): $(FILES)
	gmcs -out:$(ASSEMBLY) $(REFERENCES) -target:exe -d:DEBUG -debug $(FILES) 

clean:
	-rm -vf $(ASSEMBLY)*

new: clean all
