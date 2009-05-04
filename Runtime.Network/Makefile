ASSEMBLY = ../bin/Fmacj.Runtime.Network.dll

FILES =  \
	Communication/Client.cs \
	Communication/ExceptionResponse.cs \
	Communication/RegisterWorkClientRequest.cs \
	Communication/Request.cs \
	Communication/Response.cs \
	Communication/RunTaskRequest.cs \
	Communication/Server.cs \
	Communication/WorkClientRegisteredResponse.cs \
	Communication/WorkClientTicket.cs \
	Properties/AssemblyInfo.cs

REFERENCES = -r:../bin/Fmacj.Framework.dll

all: $(ASSEMBLY)

$(ASSEMBLY): $(FILES)
	gmcs -out:$(ASSEMBLY) $(REFERENCES) -target:library -debug $(FILES)

clean:
	-rm -vf $(ASSEMBLY)*

new: clean all
