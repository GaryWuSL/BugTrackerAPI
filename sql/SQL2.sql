select * from tblUser

UPDATE tblUser
SET PhotoFilePath = 'http://localhost:12964/api/Attachment?filename=anonymous.jpg'
WHERE UserId >= 7;


UPDATE tblUser
SET Password = 'Aa123456',
UserName = 'Rebecca Pat',
UserDescription = 'Data Entry user'
WHERE UserId = 2;


UPDATE tblUser
SET Password = 'Aa123456',
UserName = 'Rebecca Pat',
UserDescription = 'Data Entry user'
WHERE UserId = 2;


