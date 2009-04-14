ASSEMBLY = ../bin/Fmacj.Executables.FmacjRun.exe

FILES =  \
	Program.cs \
	Properties/AssemblyInfo.cs

REFERENCES =

all: $(ASSEMBLY)

$(ASSEMBLY): $(FILES)
	gmcs -out:$(ASSEMBLY) $(REFERENCES) -target:exe -debug $(FILES)

clean:
	-rm -vf $(ASSEMBLY)*

new: clean all
