��� ���������� ������� ��� ������������� ������ ����� 5 ��������������
��������� ������������ ������ ����� 4. 

��� ��� �������� � ������ ������, ������������ ��� ������ � SQLite, ������ ������������ 
� ���������� ��������, �� ��� �������� � ��������� �������

������� �������� ��������� �������

1. MusicCatalog.Context - �������� ����������� � �� SQLite
2. MusicCatalog.EntityModel - ������ ������ ��� �������������� � ���������� ��
3. MusicCatalog.WebService - ���������� REST ������� �������������� � ���������� MusicCatalog.Context, 
	��������� ������ ������ MusicCatalog.EntityModel.
	�������� ������� - https://localhost:5010/swagger/
4. MusicCatalog.Laba5 - ���������� �������������� � ������������� ��� ������ � ����������� ���������
	� ������������� ����� �������
	- � ����� json, xml, 
	- � �� SQLite (������������ �������� MusicCatalog.Context � ������ MusicCatalog.EntityModel),
	- ����� REST Service MusicCatalog.WebService
5. MusicCatalog.Test.XUnit - ��������� ����� �� ��3. 
	�������� ��������������� ���� ������������ �������� � ���������� 
	����� WebService (MusicCatalogLaba4Testing.TestWebServiceSerialization). ��� ���������� �����
	WebService ������ ���� �������.
	� ����� ��������� ����� ������ ����������� ������� - RestApiTesting �� ��������� ������� �������


����� ��������� ������������ ������ ����� 4
1. ���������� ISerializer � IMusicCatalog �������� ����������� ������
2. ����� ������ � ����������� ���������, ������� ������������� IMusicCatalog � ISerializer, ��� - MusicCatalog, ������ ������������ MCSerializerXml � MCSerializerJSon, 
������ Music�atalogRestClient, MusicCatalogSQLite �������� ��� ��������� ����������� ������ ������� ���� �����������
3. ���������� ��������� ����� ��� ������������ ����������� �������.

����� ���������� ������������ ������ ����� 5
1. ��������� ������� MusicCatalog.UI - Avalonia UI ����c������������� ���������� ��� �������������� � ������������ ����������, �������������� � ������������ ������ 4
2. �������� ������ MusicCatalog.UI.Dsktop ��� ������� ���������� MusicCatalog.UI  � ����� Windows, ��� Desktop ����������
3. �������� ��������� IMusicCatalog ��� �������������� ���������� � ������������ ������ ����� 5. ��������� ������
	- Task<IEnumerable<Composition>> Search(int stype, string query) - ��������� ������ � ��������� ���� �������
	- Task<bool> Update(Composition composition) - �������� ������������ ���������� � ��������
	- Task<bool> Delete(int id) - ������� ������������ ���������� � ��������
4. ����� ������ ���������� ����������� �� ���� �������, ������� ������������ ������� �������� � ������ ����������
	- MucisCatalog.Laba5.MusicCatalog - ����� ��� ������� �������� � ������ XML � JSON
	- MucisCatalog.Laba5.MusicCatalogRestClient - ����� ��� ������� �������� ����� WebService
	- MucisCatalog.Laba5.MusicCatalogSQLite - ����� ��� ������� �������� � SQLite
5. ��� ����������� ������� � ������ �������� � WebSerivice  ��������� ����� ������ ����������� MusicCatalog.WebService.MusicCatalogController
	- public async Task<IEnumerable<Composition>> GetCompositions(int? stype, string? query) - ��� ���������� ��� 
	  ������ ���������� ������ ����� ��������
	- public async Task<IActionResult> UpdateComposition([FromBody] Composition c) - ��� ��������� ����������
	- public async Task<IActionResult> DeleteComposition(int? id) - ��� �������� ����������
6. ��� ����������� ������ ������� � 5. ��������� ��������������� ������ � ��������� ����������� MusicCatalog.WebService.Repositories.IMusicCatalogRepository
	- Task<IEnumerable<Composition>> SearchAsync(int stype,string query) - ����� ���������� �������� ����������, ��������������� �������� ������
	- Task<bool> UpdateCompositionAsync(Composition composition) - �������� ���������� � ���������
	- Task<bool> DeleteCompositionAync(int id) - ������� ���������� �� ��������
7. ��� ���������� ������ ����������� � ������ ����������� MusicCatalog.WebService.Repositories.MusicCatalogRepository