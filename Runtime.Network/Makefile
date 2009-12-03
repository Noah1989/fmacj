
EXTRA_DIST =  Makefile

# Warning: This is an automatically generated file, do not edit!

srcdir=.
top_srcdir=..

include $(top_srcdir)/config.make

ifeq ($(CONFIG),DEBUG)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG"
ASSEMBLY = bin/Debug/Runtime.Network.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../Framework/bin/Debug/Framework.dll
BUILD_DIR = bin/Debug

FRAMEWORK_DLL_SOURCE=../Framework/bin/Debug/Framework.dll
FRAMEWORK_DLL_MDB_SOURCE=../Framework/bin/Debug/Framework.dll.mdb
FRAMEWORK_DLL_MDB=$(BUILD_DIR)/Framework.dll.mdb
RUNTIME_NETWORK_DLL_MDB_SOURCE=bin/Debug/Runtime.Network.dll.mdb
RUNTIME_NETWORK_DLL_MDB=$(BUILD_DIR)/Runtime.Network.dll.mdb

endif

ifeq ($(CONFIG),RELEASE)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize-
ASSEMBLY = bin/Release/Runtime.Network.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../Framework/bin/Release/Framework.dll
BUILD_DIR = bin/Release

FRAMEWORK_DLL_SOURCE=../Framework/bin/Release/Framework.dll
FRAMEWORK_DLL_MDB=
RUNTIME_NETWORK_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(FRAMEWORK_DLL) \
	$(FRAMEWORK_DLL_MDB) \
	$(RUNTIME_NETWORK_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(RUNTIME_NETWORK_PC)  


RESGEN=resgen2

FRAMEWORK_DLL = $(BUILD_DIR)/Framework.dll
RUNTIME_NETWORK_PC = $(BUILD_DIR)/runtime.network.pc

FILES = \
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

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	runtime.network.pc.in 

REFERENCES =  \
	System

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

#Targets
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG)  $(top_srcdir)/config.make

include $(top_srcdir)/Makefile.include
#include $(srcdir)/custom-hooks.make



$(eval $(call emit-deploy-target,FRAMEWORK_DLL))
$(eval $(call emit-deploy-target,FRAMEWORK_DLL_MDB))
$(eval $(call emit-deploy-wrapper,RUNTIME_NETWORK_PC,runtime.network.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'


$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	make pre-all-local-hook prefix=$(prefix)
	mkdir -p $(shell dirname $(ASSEMBLY))
	make $(CONFIG)_BeforeBuild
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
	make $(CONFIG)_AfterBuild
	make post-all-local-hook prefix=$(prefix)

install-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-install-local-hook prefix=$(prefix)
	make install-satellite-assemblies prefix=$(prefix)
	mkdir -p '$(DESTDIR)$(libdir)/$(PACKAGE)'
	$(call cp,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(FRAMEWORK_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(FRAMEWORK_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(RUNTIME_NETWORK_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	mkdir -p '$(DESTDIR)$(libdir)/pkgconfig'
	$(call cp,$(RUNTIME_NETWORK_PC),$(DESTDIR)$(libdir)/pkgconfig)
	make post-install-local-hook prefix=$(prefix)

uninstall-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-uninstall-local-hook prefix=$(prefix)
	make uninstall-satellite-assemblies prefix=$(prefix)
	$(call rm,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(FRAMEWORK_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(FRAMEWORK_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(RUNTIME_NETWORK_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(RUNTIME_NETWORK_PC),$(DESTDIR)$(libdir)/pkgconfig)
	make post-uninstall-local-hook prefix=$(prefix)

install: install-local
uninstall: uninstall-local
clean: clean-local

include $(top_srcdir)/rules.make
