--------------------------------------------------
--Datos
--------------------------------------------------

 
SET DEFINE OFF;
Insert into APPLICATION
   (APPLICATIONID, NAME)
 Values
   (1, 'ENTITY');
COMMIT;

SET DEFINE OFF;
Insert into ENTITY
   (ENTITYID, APPLICATIONID, NAME, TABLENAMEPREFIX)
 Values
   (1, 1, 'Rubro', ' ');
COMMIT;

SET DEFINE OFF;
Insert into ENTITYHISTORY
   (ENTITYINSTANCEID, DATEMODIFIED, ACTION, USERNAME, DATAINFO)
 Values
   (1, TO_DATE('02/25/2010 16:53:03', 'MM/DD/YYYY HH24:MI:SS'), 1, 'pnunzio', '<data RubroID="1" Description="RUBRO1" />');
Insert into ENTITYHISTORY
   (ENTITYINSTANCEID, DATEMODIFIED, ACTION, USERNAME, DATAINFO)
 Values
   (1, TO_DATE('02/25/2010 17:07:33', 'MM/DD/YYYY HH24:MI:SS'), 3, 'pnunzio', NULL);
Insert into ENTITYHISTORY
   (ENTITYINSTANCEID, DATEMODIFIED, ACTION, USERNAME, DATAINFO)
 Values
   (1, TO_DATE('02/25/2010 17:07:38', 'MM/DD/YYYY HH24:MI:SS'), 1, 'pnunzio', '<data RubroID="1" Description="RUBRO1" />');
Insert into ENTITYHISTORY
   (ENTITYINSTANCEID, DATEMODIFIED, ACTION, USERNAME, DATAINFO)
 Values
   (1, TO_DATE('02/25/2010 17:07:44', 'MM/DD/YYYY HH24:MI:SS'), 2, 'pnunzio', '<data RubroID="1" Description="RUBRO1" />');
Insert into ENTITYHISTORY
   (ENTITYINSTANCEID, DATEMODIFIED, ACTION, USERNAME, DATAINFO)
 Values
   (1, TO_DATE('02/25/2010 16:07:30', 'MM/DD/YYYY HH24:MI:SS'), 3, 'pnunzio', NULL);
Insert into ENTITYHISTORY
   (ENTITYINSTANCEID, DATEMODIFIED, ACTION, USERNAME, DATAINFO)
 Values
   (1, TO_DATE('02/25/2010 15:21:41', 'MM/DD/YYYY HH24:MI:SS'), 1, 'pnunzio', '<data RubroID="1" Description="RUBRO1" />');
Insert into ENTITYHISTORY
   (ENTITYINSTANCEID, DATEMODIFIED, ACTION, USERNAME, DATAINFO)
 Values
   (1, TO_DATE('02/25/2010 15:27:58', 'MM/DD/YYYY HH24:MI:SS'), 1, 'pnunzio', '<data RubroID="1" Description="RUBRO1" />');
Insert into ENTITYHISTORY
   (ENTITYINSTANCEID, DATEMODIFIED, ACTION, USERNAME, DATAINFO)
 Values
   (1, TO_DATE('02/25/2010 15:28:28', 'MM/DD/YYYY HH24:MI:SS'), 2, 'pnunzio', '<data RubroID="1" Description="RUBRO1 1" />');
Insert into ENTITYHISTORY
   (ENTITYINSTANCEID, DATEMODIFIED, ACTION, USERNAME, DATAINFO)
 Values
   (1, TO_DATE('02/25/2010 15:28:50', 'MM/DD/YYYY HH24:MI:SS'), 2, 'pnunzio', '<data RubroID="1" Description="RUBRO1" />');
Insert into ENTITYHISTORY
   (ENTITYINSTANCEID, DATEMODIFIED, ACTION, USERNAME, DATAINFO)
 Values
   (1, TO_DATE('02/25/2010 15:31:49', 'MM/DD/YYYY HH24:MI:SS'), 1, 'pnunzio', '<data RubroID="1" Description="RUBRO1" />');
COMMIT;

SET DEFINE OFF;
Insert into ENTITYINSTANCE
   (ENTITYINSTANCEID, ENTITYID, KEYINFO)
 Values
   (1, 1, '<key RubroID="1" />');
COMMIT;

SET DEFINE OFF;
Insert into ENTITYPROPERTY
   (ENTITYID, ISKEY, SEQUENCE, NAME)
 Values
   (1, 1, 1, 'RubroID');
Insert into ENTITYPROPERTY
   (ENTITYID, ISKEY, SEQUENCE, NAME)
 Values
   (1, 0, 1, 'RubroID');
Insert into ENTITYPROPERTY
   (ENTITYID, ISKEY, SEQUENCE, NAME)
 Values
   (1, 0, 2, 'Description');
COMMIT;
