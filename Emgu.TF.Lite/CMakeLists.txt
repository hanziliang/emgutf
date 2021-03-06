# --------------------------------------------------------
#  Copyright (C) 2004-2019 by EMGU Corporation. All rights reserved.
# --------------------------------------------------------

PROJECT(Emgu.TF.Lite)

INSTALL(
  DIRECTORY
  ${CMAKE_CURRENT_SOURCE_DIR}
  DESTINATION .
  COMPONENT emgutf_source
  FILES_MATCHING 
  PATTERN "*.cs"
  PATTERN "*.shproj"
  PATTERN "*.projitems"
  PATTERN "*.resx"
  PATTERN "README.txt"
  PATTERN ".svn" EXCLUDE
  PATTERN "obj" EXCLUDE
  PATTERN "CMakeFiles" EXCLUDE
  PATTERN "${PROJECT_NAME}.dir" EXCLUDE
  PATTERN "*Unity*" EXCLUDE
  )


IF(HAVE_WINDESKTOP_LITE)  
  PROJECT(Emgu.TF.Lite.Net45)
  #Windows Desktop
  INSTALL(
    DIRECTORY
    ${CMAKE_CURRENT_SOURCE_DIR}/Emgu.TF.Lite.Net45
    DESTINATION .
    COMPONENT emgutf_source
    FILES_MATCHING 
    PATTERN "Emgu.TF.Lite.Net45.csproj"
    PATTERN ".svn" EXCLUDE
    PATTERN "obj" EXCLUDE
    PATTERN "CMakeFiles" EXCLUDE
    PATTERN "x64" EXCLUDE #exclude build artifacts from Visual Studio build process
    PATTERN "${PROJECT_NAME}.dir" EXCLUDE
    )
  #SET(SRC_EMGU_TF_WORLD)
  #FILE(GLOB_RECURSE SRC_EMGU_TF_UTIL RELATIVE ${CMAKE_CURRENT_SOURCE_DIR} "../Emgu.TF.Util/*.cs")
  #LIST(APPEND SRC_EMGU_TF_LITE_WORLD ${SRC_EMGU_TF_UTIL})
  
  #FILE(GLOB_RECURSE SRC_EMGU_TF_LITE RELATIVE ${CMAKE_CURRENT_SOURCE_DIR} "../Emgu.TF.LITE/*.cs")
  #LIST(APPEND SRC_EMGU_TF_LITE_WORLD ${SRC_EMGU_TF_LITE})
  
  #LIST(APPEND SRC_EMGU_TF_LITE_WORLD ${CMAKE_CURRENT_SOURCE_DIR}/../CommonAssemblyInfo.cs)
  #LIST(APPEND SRC_EMGU_TF_LITE_WORLD ${CMAKE_CURRENT_SOURCE_DIR}/Properties/AssemblyInfo.cs)
  #GENERATE_DOCUMENT(${LIBRARY_OUTPUT_PATH}/${PROJECT_NAME})
  #SIGN_ASSEMBLY(${CMAKE_CURRENT_SOURCE_DIR}/../Emgu.TF.snk)
  #SET_CS_TARGET_FRAMEWORK(${CSC_TARGET_FRAMEWORK})
  #COMPILE_CS(${PROJECT_NAME} "library" "${SRC_EMGU_TF_LITE_WORLD}" ALL)
  BUILD_CSPROJ(${PROJECT_NAME} "${CMAKE_CURRENT_SOURCE_DIR}/Emgu.TF.Lite.Net45/${PROJECT_NAME}.csproj" "" ALL)
  
  INSTALL(
    FILES 
    ${LIBRARY_OUTPUT_PATH}/${PROJECT_NAME}.xml
    ${LIBRARY_OUTPUT_PATH}/${PROJECT_NAME}.dll 
    DESTINATION ${CPACK_PACKAGE_CLI_FOLDER}
    COMPONENT emgutf_binary)  
	
PROJECT(Emgu.TF.Lite.Netstandard)

#MESSAGE("In Emgu.TF.Lite.Netstandard project now. HAVE_WINDESKTOP_X64: ${HAVE_WINDESKTOP_X64}")

INSTALL(
  DIRECTORY
  ${CMAKE_CURRENT_SOURCE_DIR}/Emgu.TF.Lite.Netstandard
  DESTINATION .
  COMPONENT emgutf_source
	FILES_MATCHING 
	PATTERN "*.cs"
	PATTERN "*.projitems"
	PATTERN "*.shproj"
	PATTERN "*.csproj"
	PATTERN ".svn" EXCLUDE
	PATTERN "obj" EXCLUDE
	PATTERN "CMakeFiles" EXCLUDE
	PATTERN "x64" EXCLUDE #exclude build artifacts from Visual Studio build process
	PATTERN "x86" EXCLUDE #exclude build artifacts from Visual Studio build process
	PATTERN "${PROJECT_NAME}.dir" EXCLUDE
)

IF(WIN32 OR APPLE)
  BUILD_CSPROJ(${PROJECT_NAME} "${CMAKE_CURRENT_SOURCE_DIR}/Emgu.TF.Lite.Netstandard/Emgu.TF.Lite.Netstandard.csproj" "" ALL)

  ADD_CUSTOM_COMMAND(TARGET ${PROJECT_NAME} 
	PRE_BUILD
	COMMAND dotnet restore Emgu.TF.Lite.Netstandard.csproj
	WORKING_DIRECTORY "${CMAKE_CURRENT_SOURCE_DIR}/Emgu.TF.Lite.Netstandard")

  INSTALL(
  FILES 
  ${LIBRARY_OUTPUT_PATH}/${PROJECT_NAME}.xml
  ${LIBRARY_OUTPUT_PATH}/${PROJECT_NAME}.dll 
  DESTINATION ${CPACK_PACKAGE_CLI_FOLDER}
  COMPONENT emgutf_binary)
ENDIF()
	
ENDIF()