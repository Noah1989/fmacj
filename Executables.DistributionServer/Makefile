ASSEMBLY = ../bin/Fmacj.Executables.DistributionServer.exe

FILES =  \
	Program.cs \
	Properties/AssemblyInfo.cs \
	Server.cs

REFERENCES =  \
	-r:../bin/Fmacj.Emitter.dll \
	-r:../bin/Fmacj.Framework.dll

all: $(ASSEMBLY)

$(ASSEMBLY): $(FILES)
	gmcs -out:$(ASSEMBLY) $(REFERENCES) -target:exe -debug $(FILES)

clean:
	-rm -vf $(ASSEMBLY)*

new: clean all
