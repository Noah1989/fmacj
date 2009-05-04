ASSEMBLY = ../bin/Fmacj.Executables.WorkClient.exe

FILES =  \
	Program.cs \
	Properties/AssemblyInfo.cs \
	WorkClient.cs

REFERENCES =  \
	-r:../bin/Fmacj.Framework.dll \
	-r:../bin/Fmacj.Runtime.Network.dll

all: $(ASSEMBLY)

$(ASSEMBLY): $(FILES)
	gmcs -out:$(ASSEMBLY) $(REFERENCES) -target:exe -debug $(FILES)

clean:
	-rm -vf $(ASSEMBLY)*

new: clean all
