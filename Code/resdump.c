// resdump.c

// Dump the following PE file resources:
//
// RT_BITMAP to .bmp and .rc files
// RT_GROUP_CURSOR and RT_CURSOR to .cur and .rc files
// RT_GROUP_ICON and RT_ICON to .ico and .rc files

// NOTE: This source file was compiled using Borland C++ 4.52.

#include <dir.h>
#include <math.h>
#include <stdio.h>
#include <string.h>
#include <windows.h>

// The following four constants aren't defined in Borland C++ 4.52's windows.h
// header file.

#define RT_ANICURSOR 21
#define RT_ANIICON   22
#define RT_HTML      23
#define RT_MANIFEST  24

// As far as I know, the following six cursor and icon structure types are not
// defined by the Windows SDK.

typedef struct
{
   WORD wReserved;  // Always 0
   WORD wResID;     // Always 2
   WORD wNumImages; // Number of cursor images/directory entries
}
CURHEADER;

typedef struct
{
    WORD  wWidth;
    WORD  wHeight; // Divide by 2 to get the actual height.
    WORD  wPlanes;
    WORD  wBitCount;
    DWORD dwBytesInImage;
    WORD  wID;
}
CURDIRENTRY;

typedef struct
{
    BYTE  bWidth;         // Set to CURDIRENTRY.wHeight/2.
    BYTE  bHeight;
    BYTE  bColorCount;
    BYTE  bReserved;
    WORD  wHotspotX;
    WORD  wHotspotY;
    DWORD dwBytesInImage;
    DWORD dwImageOffset;  // Offset from start of header to the image
}
CURSORDIRENTRY;

typedef struct
{
   WORD wReserved;  // Always 0
   WORD wResID;     // Always 1
   WORD wNumImages; // Number of icon images/directory entries
}
ICOHEADER;

typedef struct
{
    BYTE  bWidth;
    BYTE  bHeight;
    BYTE  bColors;
    BYTE  bReserved;
    WORD  wPlanes;
    WORD  wBitCount;
    DWORD dwBytesInImage;
    WORD  wID;
}
ICODIRENTRY;

typedef struct
{
    BYTE  bWidth;
    BYTE  bHeight;
    BYTE  bColorCount;
    BYTE  bReserved;
    WORD  wPlanes;
    WORD  wBitCount;
    DWORD dwBytesInImage;
    DWORD dwImageOffset; // Offset from start of header to the image
}
ICONDIRENTRY;

typedef struct _RES_INFO_NODE
{
   char szResType [256];
   char szResName [256];
   void *resData;
   DWORD dwSize;
   struct _RES_INFO_NODE *next;
}
RES_INFO_NODE, *PRES_INFO_NODE;

PRES_INFO_NODE g_pResInfoNodeTop = NULL;

DWORD g_resourceSectionRVA;

void cacheResourceInfo       (void *resData, DWORD dwSize, char *szResType,
                              char *szResName);
void dumpResources           (char *szFileName);
BOOL dumpResourcesBMP        (PRES_INFO_NODE pResInfoNode);
BOOL dumpResourcesCUR        (PRES_INFO_NODE pResInfoNode);
BOOL dumpResourcesICO        (PRES_INFO_NODE pResInfoNode);
void processPEFile           (char *szFileName);
BOOL processResInstDirectory (PIMAGE_RESOURCE_DIRECTORY pResRootDir,
                              PIMAGE_RESOURCE_DIRECTORY_ENTRY pResInstDirEnt,
                              char *szResType);
BOOL processResRootDirectory (PIMAGE_RESOURCE_DIRECTORY pResRootDir);
BOOL processResTypeDirectory (PIMAGE_RESOURCE_DIRECTORY pResRootDir,
                              PIMAGE_RESOURCE_DIRECTORY_ENTRY pResTypeDirEnt);

void main (int argc, char *argv [])
{
   // Validate number of command-line arguments -- must be 2.

   if (argc != 2)
   {
       fprintf (stderr, "usage: resdump filespec\n");
       return;
   }

   // Process the presumed portable executable file identified by argv [1].
   // If processing is successful, dump found resources.

   processPEFile (argv [1]);
}

void cacheResourceInfo (void *resData, DWORD dwSize, char *szResType,
                        char *szResName)
{
   PRES_INFO_NODE pResInfoNode;
   static PRES_INFO_NODE pResInfoNodeTail;

   pResInfoNode = (PRES_INFO_NODE) malloc (sizeof (RES_INFO_NODE));
   if (pResInfoNode == NULL)
   {
       fprintf (stderr, "OUT OF MEMORY");
       exit (1);
   }

   lstrcpy (pResInfoNode->szResType, szResType);
   lstrcpy (pResInfoNode->szResName, szResName);
   pResInfoNode->resData = resData;
   pResInfoNode->dwSize = dwSize;
   pResInfoNode->next = NULL;

   if (g_pResInfoNodeTop == NULL)
       g_pResInfoNodeTop = pResInfoNode;
   else
       pResInfoNodeTail->next = pResInfoNode;

   pResInfoNodeTail = pResInfoNode;
}

void dumpResources (char *szFileName)
{
   FILE *fp;
   PRES_INFO_NODE pResInfoNode = g_pResInfoNodeTop;

   if (NULL == (fp = fopen (szFileName, "wt")))
   {
       fprintf (stderr, "unable to create %s\n", szFileName);
       return;
   }

   if (EOF == fprintf (fp, "// %s\n\n", szFileName))
   {
       fprintf (stderr, "unable to write to %s\n", szFileName);
       fclose (fp);
       return;
   }

   while (pResInfoNode != NULL)
   {
      printf ("Type = %s\n", pResInfoNode->szResType);
      printf ("Name = %s\n", pResInfoNode->szResName);
      printf ("Address = %x\n", pResInfoNode->resData);
      printf ("Size = %d\n\n", pResInfoNode->dwSize);

      if (!strcmp (pResInfoNode->szResType, "RT_BITMAP"))
      {
          if (EOF == fprintf (fp, "%s BITMAP \"%s.bmp\"\n\n",
                              pResInfoNode->szResName,
                              pResInfoNode->szResName))
          {
              fprintf (stderr, "unable to write to %s\n", szFileName);
              break;
          }

          if (!dumpResourcesBMP (pResInfoNode))
              break;
      }
      else
      if (!strcmp (pResInfoNode->szResType, "RT_GROUP_CURSOR"))
      {
          if (EOF == fprintf (fp, "%s CURSOR \"%s.cur\"\n\n",
                              pResInfoNode->szResName,
                              pResInfoNode->szResName))
          {
              fprintf (stderr, "unable to write to %s\n", szFileName);
              break;
          }

          if (!dumpResourcesCUR (pResInfoNode))
              break;
      }
      else
      if (!strcmp (pResInfoNode->szResType, "RT_GROUP_ICON"))
      {
          if (EOF == fprintf (fp, "%s ICON \"%s.ico\"\n\n",
                              pResInfoNode->szResName,
                              pResInfoNode->szResName))
          {
              fprintf (stderr, "unable to write to %s\n", szFileName);
              break;
          }

          if (!dumpResourcesICO (pResInfoNode))
              break;
      }

      pResInfoNode = pResInfoNode->next;
   }

   fclose (fp);

   // Free the memory allocated to the linked list.

   pResInfoNode = g_pResInfoNodeTop;
   while (pResInfoNode != NULL)
   {
      PRES_INFO_NODE pResInfoNodeTemp = pResInfoNode;
      pResInfoNode = pResInfoNode->next;
      free (pResInfoNodeTemp);
   }
}

BOOL dumpResourcesBMP (PRES_INFO_NODE pResInfoNode)
{
   BITMAPFILEHEADER bmfh;
   BITMAPINFOHEADER *pbmih;
   char filename [256];
   FILE *fp;

   wsprintf (filename, "%s.bmp", strlwr (pResInfoNode->szResName));
   if (NULL == (fp = fopen (filename, "wb")))
   {
       fprintf (stderr, "unable to create %s\n", filename);
       return FALSE;
   }

   bmfh.bfType = 'M'*256+'B';
   bmfh.bfSize = pResInfoNode->dwSize+sizeof (BITMAPFILEHEADER);
   bmfh.bfReserved1 = 0;
   bmfh.bfReserved2 = 0;
   bmfh.bfOffBits = sizeof (BITMAPFILEHEADER)+sizeof (BITMAPINFOHEADER);

   // Account for color table (if present).

   pbmih = (BITMAPINFOHEADER *) pResInfoNode->resData;

   if (pbmih->biClrUsed != 0)
       bmfh.bfOffBits += 4*pbmih->biClrUsed;
   else
   if (pbmih->biBitCount <= 8)
       bmfh.bfOffBits += 4*(int) pow (2, pbmih->biBitCount);

   if (fwrite (&bmfh, sizeof (BITMAPFILEHEADER), 1, fp) != 1)
   {
       fprintf (stderr, "unable to write BITMAPFILEHEADER to %s\n", filename);
       fclose (fp);
       return FALSE;
   }

   if (fwrite (pResInfoNode->resData, pResInfoNode->dwSize, 1, fp) != 1)
   {
       fprintf (stderr, "unable to write bitmap data to %s\n", filename);
       fclose (fp);
       return FALSE;
   }

   fclose (fp);
   return TRUE;
}

BOOL dumpResourcesCUR (PRES_INFO_NODE pResInfoNode)
{
   char filename [256];
   CURDIRENTRY *pcurde;
   CURHEADER *pcurh;
   CURSORDIRENTRY cursorde;
   DWORD dwOffset;
   FILE *fp;
   PRES_INFO_NODE *pResInfoNodeRT_CURSOR;
   WORD w;

   wsprintf (filename, "%s.cur", strlwr (pResInfoNode->szResName));
   if (NULL == (fp = fopen (filename, "wb")))
   {
       fprintf (stderr, "unable to create %s\n", filename);
       return FALSE;
   }

   pcurh = (CURHEADER *) pResInfoNode->resData;
   if (fwrite (pcurh, sizeof (CURHEADER), 1, fp) != 1)
   {
       fprintf (stderr, "unable to write CURHEADER to %s\n", filename);
       fclose (fp);
       return FALSE;
   }

   pResInfoNodeRT_CURSOR = (PRES_INFO_NODE *) malloc (pcurh->wNumImages*
                                                      sizeof (PRES_INFO_NODE));
   if (pResInfoNodeRT_CURSOR == NULL)
   {
       fprintf (stderr, "OUT OF MEMORY");
       exit (1);
   }

   dwOffset = sizeof (CURHEADER)+pcurh->wNumImages*sizeof (CURSORDIRENTRY);

   for (w = 0; w < pcurh->wNumImages; w++)
   {
        pcurde = (CURDIRENTRY *) (((char *) pResInfoNode->resData)+
                 sizeof (CURHEADER)+w*sizeof (CURDIRENTRY));
        cursorde.bWidth = (BYTE) pcurde->wWidth;
        cursorde.bHeight = (BYTE) (pcurde->wHeight/2); // Need to divide by 2.
        cursorde.bColorCount = 0; // It's okay to set this field to 0.
        cursorde.bReserved = 0;

        {
           char buffer [256];
           PRES_INFO_NODE pResInfoNodeTemp = g_pResInfoNodeTop;

           while (pResInfoNodeTemp != NULL)
           {
              wsprintf (buffer, "%d", pcurde->wID);

              if (!lstrcmp (pResInfoNodeTemp->szResType, "RT_CURSOR") &&
                  !lstrcmp (pResInfoNodeTemp->szResName, buffer))
              {
                  cursorde.wHotspotX = *(WORD *) pResInfoNodeTemp->resData;
                  cursorde.wHotspotY = *(WORD *)
                                       (2+(char *) pResInfoNodeTemp->resData);
                  pResInfoNodeRT_CURSOR [w] = pResInfoNodeTemp;
                  break;
              }

              pResInfoNodeTemp = pResInfoNodeTemp->next;
           }

           if (pResInfoNodeTemp == NULL)
           {
               fprintf (stderr, "CORRUPT PE FILE");
               exit (1);
           }
        }

        cursorde.dwBytesInImage = pcurde->dwBytesInImage-4; // Subtract 4
        cursorde.dwImageOffset = dwOffset; // bytes for X&Y hotspot fields.

        if (fwrite (&cursorde, sizeof (CURSORDIRENTRY), 1, fp) != 1)
        {
            fprintf (stderr, "unable to write CURSORDIRENTRY to %s\n",
                     filename);
            free (pResInfoNodeRT_CURSOR);
            fclose (fp);
            return FALSE;
        }

        dwOffset += cursorde.dwBytesInImage;
   }

   for (w = 0; w < pcurh->wNumImages; w++)
   {
        // Add 4 bytes to skip past X&Y hotspot fields. Subtract 4 bytes 
        // because these fields are not written with the rest of the cursor 
        // image to the .cur file.

        if (fwrite (4+(char *) pResInfoNodeRT_CURSOR [w]->resData,
                    pResInfoNodeRT_CURSOR [w]->dwSize-4, 1, fp) != 1)
        {
            fprintf (stderr, "unable to write cursor %d to %s\n", w, filename);
            free (pResInfoNodeRT_CURSOR);
            fclose (fp);
            return FALSE;
        }
   }

   free (pResInfoNodeRT_CURSOR);
   fclose (fp);
   return TRUE;
}

BOOL dumpResourcesICO (PRES_INFO_NODE pResInfoNode)
{
   char filename [256];
   DWORD dwOffset;
   FILE *fp;
   ICODIRENTRY *picode;
   ICOHEADER *picoh;
   ICONDIRENTRY iconde;
   WORD w, *pwIDs;
   
   wsprintf (filename, "%s.ico", strlwr (pResInfoNode->szResName));
   if (NULL == (fp = fopen (filename, "wb")))
   {
       fprintf (stderr, "unable to create %s\n", filename);
       return FALSE;
   }

   picoh = (ICOHEADER *) pResInfoNode->resData;
   if (fwrite (picoh, sizeof (ICOHEADER), 1, fp) != 1)
   {
       fprintf (stderr, "unable to write ICOHEADER to %s\n", filename);
       fclose (fp);
       return FALSE;
   }

   pwIDs = (WORD *) malloc (picoh->wNumImages*sizeof (WORD));
   if (pwIDs == NULL)
   {
       fprintf (stderr, "OUT OF MEMORY");
       exit (1);
   }

   dwOffset = sizeof (ICOHEADER)+picoh->wNumImages*sizeof (ICONDIRENTRY);

   for (w = 0; w < picoh->wNumImages; w++)
   {
        picode = (ICODIRENTRY *) (((char *) pResInfoNode->resData)+
                 sizeof (ICOHEADER)+w*sizeof (ICODIRENTRY));

        iconde.bWidth = picode->bWidth;
        iconde.bHeight = picode->bHeight;
        iconde.bColorCount = picode->bColors;
        iconde.bReserved = picode->bReserved;
        iconde.wPlanes = picode->wPlanes;
        iconde.wBitCount = picode->wBitCount;
        iconde.dwBytesInImage = picode->dwBytesInImage;
        iconde.dwImageOffset = dwOffset;

        if (fwrite (&iconde, sizeof (ICONDIRENTRY), 1, fp) != 1)
        {
            fprintf (stderr, "unable to write ICONDIRENTRY to %s\n", filename);
            free (pwIDs);
            fclose (fp);
            return FALSE;
        }

        pwIDs [w] = picode->wID;
        dwOffset += iconde.dwBytesInImage;
   }

   for (w = 0; w < picoh->wNumImages; w++)
   {
        char buffer [256];
        PRES_INFO_NODE pResInfoNodeTemp = g_pResInfoNodeTop;

        while (pResInfoNodeTemp != NULL)
        {
           wsprintf (buffer, "%d", pwIDs [w]);
           if (!lstrcmp (pResInfoNodeTemp->szResType, "RT_ICON") &&
               !lstrcmp (pResInfoNodeTemp->szResName, buffer))
           {
               if (fwrite (pResInfoNodeTemp->resData, pResInfoNodeTemp->dwSize,
                   1, fp) != 1)
               {
                   fprintf (stderr, "unable to write icon %d to %s\n", w,
                            filename);
                   free (pwIDs);
                   fclose (fp);
                   return FALSE;
               }
               break;
           }

           pResInfoNodeTemp = pResInfoNodeTemp->next;               
        }

        if (pResInfoNodeTemp == NULL)
        {
            fprintf (stderr, "CORRUPT PE FILE");
            exit (1);
        }
   }

   free (pwIDs);
   fclose (fp);
   return TRUE;
}

void processPEFile (char *szFileName)
{
   char dir [MAXDIR], drive [MAXDRIVE], ext [MAXEXT], file [MAXFILE];
   char szRCName [MAXPATH];
   HANDLE hFile, hFileMapping;
   int i;
   LPVOID lpFileBase;
   PIMAGE_DOS_HEADER pDosHeader;
   PIMAGE_NT_HEADERS pNtHeaders;
   PIMAGE_RESOURCE_DIRECTORY pResRootDir;
   PIMAGE_SECTION_HEADER pSectionHeader;

   // Attempt to open the EXE file.

   hFile = CreateFile (szFileName, GENERIC_READ, FILE_SHARE_READ, NULL,
                       OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, 0);

   // Exit if file could not be opened.

   if (hFile == INVALID_HANDLE_VALUE)
   {
       fprintf (stderr, "resdump: could not open %s\n", szFileName);
       return;
   }

   // Attempt to create a mapping so that the file can be traversed.

   hFileMapping = CreateFileMapping (hFile, NULL, PAGE_READONLY, 0, 0, NULL);

   // Exit if mapping could not be created.

   if (hFileMapping == 0)
   {
       fprintf (stderr, "resdump: unable to create file mapping\n");
       goto processPEFile1;
   }

   // Attempt to map a view of the file into the resdump process's address
   // space.

   lpFileBase = MapViewOfFile (hFileMapping, FILE_MAP_READ, 0, 0, 0);

   // Exit if mapping not possible.

   if (lpFileBase == 0)
   {
       fprintf (stderr, "resdump: unable to map view of file\n");
       goto processPEFile2;
   }

   // Obtain address of DOS header MZ signature.

   pDosHeader = (PIMAGE_DOS_HEADER) lpFileBase;

   // Exit if signature not present.

   if (pDosHeader->e_magic != IMAGE_DOS_SIGNATURE)
   {
       fprintf (stderr, "resdump: DOS header missing\n");
       goto processPEFile3;
   }

   // Exit if file too small to have PE header.

   if (pDosHeader->e_lfanew >= GetFileSize (hFile, NULL))
   {
       fprintf (stderr, "resdump: not a PE-based EXE file\n");
       goto processPEFile3;
   }

   // Must use (char *) cast so that only bytes -- not multiples of
   // IMAGE_DOS_HEADER -- are added to pDosHeader.

   pNtHeaders = (PIMAGE_NT_HEADERS)
                ((char *) pDosHeader+pDosHeader->e_lfanew);

   // Exit if no PE signature.

   if (pNtHeaders->Signature != IMAGE_NT_SIGNATURE)
   {
       fprintf (stderr, "resdump: not a PE file\n");
       goto processPEFile3;
   }

   // Search for .rsrc section.

   pSectionHeader = IMAGE_FIRST_SECTION (pNtHeaders);
   for (i = 0; i < pNtHeaders->FileHeader.NumberOfSections; i++)
        if (!strncmp ((char *) pSectionHeader->Name, ".rsrc", 5))
        {
            // Found!

            // Reference the resource section's root directory.

            pResRootDir = (PIMAGE_RESOURCE_DIRECTORY)
                          ((char *) pDosHeader+
                          pSectionHeader->PointerToRawData);

            g_resourceSectionRVA = pSectionHeader->VirtualAddress;

            // Process root directory.

            processResRootDirectory (pResRootDir);

            break;
        }
        else
            pSectionHeader++; // Point to next section header.

   fnsplit (szFileName, drive, dir, file, ext);
   wsprintf (szRCName, "%s.rc", file);
   dumpResources (szRCName);

processPEFile3:

   // Unmap the mapped view.

   UnmapViewOfFile (lpFileBase);

processPEFile2:                                    

   // Close the mapping.

   CloseHandle (hFileMapping);

processPEFile1:

   // Close the file.

   CloseHandle (hFile);
}

BOOL processResInstDirectory (PIMAGE_RESOURCE_DIRECTORY pResRootDir,
                              PIMAGE_RESOURCE_DIRECTORY_ENTRY pResInstDirEnt,
                              char *szResType)
{
   char szResName [256];
   int i, nEntries;
   PIMAGE_RESOURCE_DATA_ENTRY pResDataDir;
   PIMAGE_RESOURCE_DIRECTORY pResInstDir;
   PIMAGE_RESOURCE_DIRECTORY_ENTRY pResDataDirEnt;
   PIMAGE_RESOURCE_DIR_STRING_U pDirStringU;

   // Identify resource instance directory entry by name or ID.

   if (pResInstDirEnt->u.s.NameIsString)
   {
       pDirStringU = (PIMAGE_RESOURCE_DIR_STRING_U)
                     ((char *) pResRootDir+pResInstDirEnt->u.s.NameOffset);

       // Convert from wide characters to ANSI characters.

       WideCharToMultiByte (CP_ACP, 0, (LPCWSTR) pDirStringU->NameString,
                            pDirStringU->Length, szResName, 255, NULL, NULL);
       szResName [pDirStringU->Length] = '\0';
   }
   else
       wsprintf (szResName, "%d", pResInstDirEnt->u.Id);

   // A resource instance directory entry must point to a resource instance
   // directory.

   if (!pResInstDirEnt->u2.s.DataIsDirectory)
   {
       fprintf (stderr, "resdump: instance directory expected\n");
       return FALSE;
   }

   // Obtain resource instance directory address.

   pResInstDir = (PIMAGE_RESOURCE_DIRECTORY)
                 ((char *) pResRootDir+pResInstDirEnt->u2.s.OffsetToDirectory);

   // Reference the directory entries array that follows the NumberOfIdEntries
   // field in the IMAGE_RESOURCE_DIRECTORY structure.

   pResDataDirEnt = (PIMAGE_RESOURCE_DIRECTORY_ENTRY) (pResInstDir+1);

   // Calculate the number of entries in the array.

   nEntries = pResInstDir->NumberOfNamedEntries;
   nEntries += pResInstDir->NumberOfIdEntries;

   // Process all entries. Each entry must point to a resource data entry.

   for (i = 0; i < nEntries; i++)
   {
        pResDataDir = (PIMAGE_RESOURCE_DATA_ENTRY)
                      ((char *) pResRootDir+pResDataDirEnt->u2.OffsetToData);

        cacheResourceInfo (((char *) pResRootDir)+pResDataDir->OffsetToData-
                           g_resourceSectionRVA, pResDataDir->Size, szResType,
                           szResName);

        // Reference next entry in directory entries array.

        pResDataDirEnt++; // Should only be one entry.
   }

   return TRUE;
}

BOOL processResRootDirectory (PIMAGE_RESOURCE_DIRECTORY pResRootDir)
{
   int i, nEntries;
   PIMAGE_RESOURCE_DIRECTORY_ENTRY pResTypeDirEnt;

   // Reference the directory entries array that follows the NumberOfIdEntries
   // field in the IMAGE_RESOURCE_DIRECTORY structure.

   pResTypeDirEnt = (PIMAGE_RESOURCE_DIRECTORY_ENTRY) (pResRootDir+1);

   // Calculate the number of entries in the array.

   nEntries = pResRootDir->NumberOfNamedEntries;
   nEntries += pResRootDir->NumberOfIdEntries;

   // Process all entries. Each entry must point to a resource type directory.

   for (i = 0; i < nEntries; i++)
   {
        if (!processResTypeDirectory (pResRootDir, pResTypeDirEnt))
            return FALSE;

        // Reference next entry in directory entries array.

        pResTypeDirEnt++;
   }

   return TRUE;
}

BOOL processResTypeDirectory (PIMAGE_RESOURCE_DIRECTORY pResRootDir,
                              PIMAGE_RESOURCE_DIRECTORY_ENTRY pResTypeDirEnt)
{
   char szResType [256];
   int i, nEntries;
   PIMAGE_RESOURCE_DIRECTORY pResTypeDir;
   PIMAGE_RESOURCE_DIRECTORY_ENTRY pResInstDirEnt;
   PIMAGE_RESOURCE_DIR_STRING_U pDirStringU;

   // Identify resource type directory entry by name or ID. Will most likely
   // be identified by ID.

   if (pResTypeDirEnt->u.s.NameIsString)
   {
       pDirStringU = (PIMAGE_RESOURCE_DIR_STRING_U)
                     ((char *) pResRootDir+pResTypeDirEnt->u.s.NameOffset);

       // Convert from wide characters to ANSI characters.

       WideCharToMultiByte (CP_ACP, 0, (LPCWSTR) pDirStringU->NameString,
                            pDirStringU->Length, szResType, 255, NULL, NULL);
       szResType [pDirStringU->Length] = '\0';
   }
   else
       switch (pResTypeDirEnt->u.Id)
       {
          case RT_ACCELERATOR:
               lstrcpy (szResType, "RT_ACCELERATOR");
               break;

          case RT_ANICURSOR:
               lstrcpy (szResType, "RT_ANICURSOR");
               break;

          case RT_ANIICON:
               lstrcpy (szResType, "RT_ANIICON");
               break;

          case RT_BITMAP:
               lstrcpy (szResType, "RT_BITMAP");
               break;

          case RT_CURSOR:
               lstrcpy (szResType, "RT_CURSOR");
               break;

          case RT_DIALOG:
               lstrcpy (szResType, "RT_DIALOG");
               break;

          case RT_DLGINCLUDE:
               lstrcpy (szResType, "RT_DLGINCLUDE");
               break;

          case RT_FONT:
               lstrcpy (szResType, "RT_FONT");
               break;

          case RT_FONTDIR:
               lstrcpy (szResType, "RT_FONTDIR");
               break;

          case RT_GROUP_CURSOR:
               lstrcpy (szResType, "RT_GROUP_CURSOR");
               break;

          case RT_GROUP_ICON:
               lstrcpy (szResType, "RT_GROUP_ICON");
               break;

          case RT_HTML:
               lstrcpy (szResType, "RT_HTML");
               break;

          case RT_ICON:
               lstrcpy (szResType, "RT_ICON");
               break;

          case RT_MANIFEST:
               lstrcpy (szResType, "RT_MANIFEST");
               break;

          case RT_MENU:
               lstrcpy (szResType, "RT_MENU");
               break;

          case RT_MESSAGETABLE:
               lstrcpy (szResType, "RT_MESSAGETABLE");
               break;

          case RT_PLUGPLAY:
               lstrcpy (szResType, "RT_PLUGPLAY");
               break;

          case RT_RCDATA:
               lstrcpy (szResType, "RT_RCDATA");
               break;

          case RT_STRING:
               lstrcpy (szResType, "RT_STRING");
               break;

          case RT_VERSION:
               lstrcpy (szResType, "RT_VERSION");
               break;

          case RT_VXD:
               lstrcpy (szResType, "RT_VXD");
               break;

          default: wsprintf (szResType, "UNKNOWN ID: %d");
       }

   // A resource type directory entry must point to a resource type directory.

   if (!pResTypeDirEnt->u2.s.DataIsDirectory)
   {
       fprintf (stderr, "resdump: type directory expected\n");
       return FALSE;
   }

   // Obtain resource type directory address.

   pResTypeDir = (PIMAGE_RESOURCE_DIRECTORY)
                 ((char *) pResRootDir+pResTypeDirEnt->u2.s.OffsetToDirectory);

   // Reference the directory entries array that follows the NumberOfIdEntries
   // field in the IMAGE_RESOURCE_DIRECTORY structure.

   pResInstDirEnt = (PIMAGE_RESOURCE_DIRECTORY_ENTRY) (pResTypeDir+1);

   // Calculate the number of entries in the array.

   nEntries = pResTypeDir->NumberOfNamedEntries;
   nEntries += pResTypeDir->NumberOfIdEntries;

   // Process all entries. Each entry must point to a resource instance
   // directory.

   for (i = 0; i < nEntries; i++)
   {
        if (!processResInstDirectory (pResRootDir, pResInstDirEnt, szResType))
            return FALSE;

        // Reference next entry in directory entries array.

        pResInstDirEnt++;
   }

   return TRUE;
}

